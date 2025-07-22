# Hướng dẫn Setup CI/CD với GitHub Actions và Azure

## Mục lục

1. [Tổng quan](#1-tổng-quan)
2. [Chuẩn bị](#2-chuẩn-bị)
3. [Bước 1: Tạo Dockerfile](#bước-1-tạo-dockerfile)
4. [Bước 2: Tạo Azure Container Registry (ACR)](#bước-2-tạo-azure-container-registry-acr)
5. [Bước 3: Tạo Azure App Service](#bước-3-tạo-azure-app-service)
6. [Bước 4: Setup Managed Identity](#bước-4-setup-managed-identity)
7. [Bước 5: Tạo Service Principal cho GitHub Actions](#bước-5-tạo-service-principal-cho-github-actions)
8. [Bước 6: Thêm Secrets vào GitHub](#bước-6-thêm-secrets-vào-github)
9. [Bước 7: Tạo GitHub Actions Workflow](#bước-7-tạo-github-actions-workflow)
10. [Bước 8: Config App Settings](#bước-8-config-app-settings)
11. [Bước 9: Test CI/CD](#bước-9-test-cicd)
12. [Troubleshooting](#troubleshooting)

---

## 1. Tổng quan

### CI/CD là gì?

- **CI (Continuous Integration)**: Tự động build và test code mỗi khi có thay đổi
- **CD (Continuous Deployment)**: Tự động deploy code lên server sau khi build thành công

### Luồng hoạt động

```
1. Developer push code → GitHub
2. GitHub Actions tự động:
   ├─ Build Docker image
   ├─ Push image lên Azure Container Registry
   └─ Deploy image lên Azure App Service
3. ✅ App tự động cập nhật trên production
```

### Các dịch vụ sử dụng

- **GitHub Actions**: CI/CD runner (miễn phí cho public repo)
- **Azure Container Registry (ACR)**: Lưu trữ Docker images
- **Azure App Service**: Hosting ứng dụng web

---

## 2. Chuẩn bị

### Yêu cầu

- ✅ Tài khoản GitHub (có repository chứa code)
- ✅ Tài khoản Azure
- ✅ Azure CLI đã cài đặt (hoặc dùng Cloud Shell)
- ✅ Code đã có trên GitHub

### Kiểm tra Azure CLI

```bash
# Kiểm tra version
az --version

# Login vào Azure
az login
```

**Nếu chưa có Azure CLI**, dùng **Azure Cloud Shell**:

1. Truy cập: <https://portal.azure.com/#cloudshell/>
2. Chọn **Bash**
3. Chạy các lệnh trong hướng dẫn này

---

## Bước 1: Tạo Dockerfile

### 1.1. Tạo file `Dockerfile` ở root của project

**Ví dụ cho ASP.NET Core:**

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

COPY . /source
WORKDIR /source/TalkingTails.API

# Build application
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy compiled app
COPY --from=build /app .

# Expose port
EXPOSE 8080

# Set environment variable for port
ENV ASPNETCORE_URLS=http://+:8080

# Run app
ENTRYPOINT ["dotnet", "TalkingTails.API.dll"]
```

### 1.2. Test Docker build local (optional)

```bash
# Build image
docker build -t myapp:test .

# Run container
docker run -p 8080:8080 myapp:test

# Test
curl http://localhost:8080
```

---

## Bước 2: Tạo Azure Container Registry (ACR)

### 2.1. Lấy Subscription ID

```bash
# Lấy subscription ID (cần cho các bước sau)
az account show --query id -o tsv
```

**Lưu lại giá trị này**, ví dụ: `608054e7-a4c5-8912-b1de-842b514ggcff`

### 2.2. Tạo Resource Group

```bash
# Thay <ResourceGroupName> bằng tên bạn muốn (VD: MyAppRG)
# Thay <Location> bằng region (VD: southeastasia, eastus)
az group create \
  --name <ResourceGroupName> \
  --location <Location>
```

**Ví dụ:**

```bash
az group create \
  --name TalkingTails \
  --location southeastasia
```

### 2.3. Tạo Container Registry

```bash
# Thay <ACRName> bằng tên registry (chỉ chữ thường, số, không dấu)
# Lưu ý: Tên phải unique trên toàn Azure
az acr create \
  --name <ACRName> \
  --resource-group <ResourceGroupName> \
  --sku Basic \
  --admin-enabled true
```

**Ví dụ:**

```bash
az acr create \
  --name talkingtailsacr \
  --resource-group TalkingTails \
  --sku Basic \
  --admin-enabled true
```

### 2.4. Lấy ACR credentials

```bash
# Lấy username
az acr credential show \
  --name <ACRName> \
  --query username -o tsv

# Lấy password
az acr credential show \
  --name <ACRName> \
  --query passwords[0].value -o tsv
```

**Lưu lại 2 giá trị này** - sẽ dùng cho GitHub Secrets.

---

## Bước 3: Tạo Azure App Service

### 3.1. Tạo App Service Plan

```bash
# Tạo plan với Linux + Docker support
az appservice plan create \
  --name <PlanName> \
  --resource-group <ResourceGroupName> \
  --is-linux \
  --sku B1
```

**Giải thích SKU:**

- `F1`: Free (không support Docker)
- `B1`: Basic ($13/tháng) - **Khuyến nghị cho dev/test**
- `S1`: Standard ($70/tháng)
- `P1V2`: Premium ($85/tháng)

**Ví dụ:**

```bash
az appservice plan create \
  --name talkingtails-plan \
  --resource-group TalkingTails \
  --is-linux \
  --sku B1
```

### 3.2. Tạo Web App

```bash
# Tạo web app với container placeholder
az webapp create \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --plan <PlanName> \
  --deployment-container-image-name nginx:latest
```

**Lưu ý:** `<AppName>` phải unique trên toàn Azure (sẽ thành URL: `https://<AppName>.azurewebsites.net`)

**Ví dụ:**

```bash
az webapp create \
  --name talkingtails \
  --resource-group TalkingTails \
  --plan talkingtails-plan \
  --deployment-container-image-name nginx:latest
```

---

## Bước 4: Setup Managed Identity

### 4.1. Enable System-Assigned Managed Identity

```bash
az webapp identity assign \
  --name <AppName> \
  --resource-group <ResourceGroupName>
```

### 4.2. Lấy Principal ID

```bash
PRINCIPAL_ID=$(az webapp identity show \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --query principalId -o tsv)

echo $PRINCIPAL_ID
```

**Lưu lại giá trị này** nếu cần debug sau này.

### 4.3. Gán quyền AcrPull cho App Service

```bash
# Lấy subscription ID nếu chưa có
SUBSCRIPTION_ID=$(az account show --query id -o tsv)

# Gán quyền
az role assignment create \
  --assignee $PRINCIPAL_ID \
  --role AcrPull \
  --scope /subscriptions/$SUBSCRIPTION_ID/resourceGroups/<ResourceGroupName>/providers/Microsoft.ContainerRegistry/registries/<ACRName>
```

**Ví dụ đầy đủ:**

```bash
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
PRINCIPAL_ID=$(az webapp identity show \
  --name talkingtails \
  --resource-group TalkingTails \
  --query principalId -o tsv)

az role assignment create \
  --assignee $PRINCIPAL_ID \
  --role AcrPull \
  --scope /subscriptions/$SUBSCRIPTION_ID/resourceGroups/TalkingTails/providers/Microsoft.ContainerRegistry/registries/talkingtailsacr
```

### 4.4. Bật Managed Identity cho ACR

```bash
az webapp config set \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --generic-configurations '{"acrUseManagedIdentityCreds": true}'
```

**Ví dụ:**

```bash
az webapp config set \
  --name talkingtails \
  --resource-group TalkingTails \
  --generic-configurations '{"acrUseManagedIdentityCreds": true}'
```

---

## Bước 5: Tạo Service Principal cho GitHub Actions

### 5.1. Tạo Service Principal

```bash
# Lấy subscription ID
SUBSCRIPTION_ID=$(az account show --query id -o tsv)

# Tạo service principal với quyền Contributor
az ad sp create-for-rbac \
  --name "github-actions-<AppName>" \
  --role Contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID/resourceGroups/<ResourceGroupName> \
  --sdk-auth
```

**Ví dụ:**

```bash
SUBSCRIPTION_ID=$(az account show --query id -o tsv)

az ad sp create-for-rbac \
  --name "github-actions-talkingtails" \
  --role Contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID/resourceGroups/TalkingTails \
  --sdk-auth
```

### 5.2. Lưu output JSON

Output sẽ có dạng:

```json
{
  "clientId": "12345678-1234-1234-1234-123456789abc",
  "clientSecret": "abcdef~xxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "subscriptionId": "408054e7-a4c5-4825-b1de-159b722ffcff",
  "tenantId": "87654321-4321-4321-4321-cba987654321",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  ...
}
```

**⚠️ QUAN TRỌNG:** Copy **TOÀN BỘ** JSON này - sẽ dùng cho GitHub Secret `AZURE_CREDENTIALS`.

---

## Bước 6: Thêm Secrets vào GitHub

### 6.1. Truy cập GitHub Secrets

1. Vào repository trên GitHub
2. Click **Settings** (tab trên cùng)
3. Bên trái chọn **Secrets and variables** → **Actions**
4. Click **New repository secret**

### 6.2. Thêm 3 secrets sau

#### Secret 1: AZURE_CREDENTIALS

- **Name:** `AZURE_CREDENTIALS`
- **Value:** Toàn bộ JSON từ bước 5.2 (service principal)

```json
{
  "clientId": "...",
  "clientSecret": "...",
  "subscriptionId": "...",
  "tenantId": "..."
}
```

#### Secret 2: ACR_USERNAME

- **Name:** `ACR_USERNAME`
- **Value:** Username từ bước 2.4 (thường là tên ACR)

```
talkingtailsacr
```

#### Secret 3: ACR_PASSWORD

- **Name:** `ACR_PASSWORD`
- **Value:** Password từ bước 2.4

```
abc123def456...
```

### 6.3. Xác nhận

Sau khi thêm, bạn sẽ thấy 3 secrets:

```
AZURE_CREDENTIALS     ••••••••
ACR_USERNAME          ••••••••
ACR_PASSWORD          ••••••••
```

---

## Bước 7: Tạo GitHub Actions Workflow

### 7.1. Tạo thư mục workflow

```bash
mkdir -p .github/workflows
```

### 7.2. Tạo file `.github/workflows/ci-cd.yml`

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

env:
  # Thay đổi theo ACR và app name của bạn
  DOCKER_IMAGE: <ACRName>.azurecr.io/<ImageName>

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    steps:
      # Checkout code từ repository
      - uses: actions/checkout@v4

      # Login vào Azure (để có quyền truy cập các resources)
      - name: Login to Azure
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      # Login vào Azure Container Registry
      - name: Login to ACR
        uses: azure/docker-login@v2
        with:
          login-server: <ACRName>.azurecr.io
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}

      # Build và push Docker image
      - name: Build and push Docker image
        uses: docker/build-push-action@v6
        with:
          context: .
          # Chỉ push khi merge vào main (không push với PR)
          push: ${{ github.ref == 'refs/heads/main' }}
          tags: |
            ${{ env.DOCKER_IMAGE }}:${{ github.sha }}
            ${{ env.DOCKER_IMAGE }}:latest

  deploy:
    needs: build-and-push
    # Chỉ deploy khi push vào main (không deploy PR)
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      # Login vào Azure
      - uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      # Deploy lên Azure App Service
      - name: Deploy to Azure App Service
        uses: azure/webapps-deploy@v3
        with:
          app-name: <AppName>
          images: ${{ env.DOCKER_IMAGE }}:${{ github.sha }}
```

### 7.3. Thay thế các placeholder

**Thay đổi những giá trị sau:**

| Placeholder   | Thay bằng          | Ví dụ              |
| ------------- | ------------------ | ------------------ |
| `<ACRName>`   | Tên ACR của bạn    | `talkingtailsacr`  |
| `<ImageName>` | Tên image bạn muốn | `talkingtails-api` |
| `<AppName>`   | Tên App Service    | `talkingtails`     |

**Ví dụ file hoàn chỉnh:**

```yaml
name: CI/CD TalkingTails API

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

env:
  DOCKER_IMAGE: talkingtailsacr.azurecr.io/talkingtails-api

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Login to Azure
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Login to ACR
        uses: azure/docker-login@v2
        with:
          login-server: talkingtailsacr.azurecr.io
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}

      - name: Build and push Docker image
        uses: docker/build-push-action@v6
        with:
          context: .
          push: ${{ github.ref == 'refs/heads/main' }}
          tags: |
            ${{ env.DOCKER_IMAGE }}:${{ github.sha }}
            ${{ env.DOCKER_IMAGE }}:latest

  deploy:
    needs: build-and-push
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      - uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Deploy to Azure App Service
        uses: azure/webapps-deploy@v3
        with:
          app-name: talkingtails
          images: ${{ env.DOCKER_IMAGE }}:${{ github.sha }}
```

---

## Bước 8: Config App Settings

### 8.1. Thay vì commit appsettings.json, dùng Azure App Settings

```bash
# Set environment variables trên Azure
az webapp config appsettings set \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    ConnectionStrings__DefaultConnection="Server=..." \
    ApiKeys__OpenAI="sk-..." \
    WEBSITES_PORT="8080"
```

### 8.2. Ví dụ cụ thể

**Cho ASP.NET Core:**

```bash
az webapp config appsettings set \
  --name talkingtails \
  --resource-group TalkingTails \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    ASPNETCORE_URLS="http://+:8080" \
    ConnectionStrings__Database="Server=mydb.database.windows.net;Database=mydb;User Id=admin;Password=YourStrongPasswordHere" \
    JWT__SecretKey="your-jwt-secret-key-min-32-chars" \
    WEBSITES_PORT="8080"
```

### 8.3. Xem App Settings hiện tại

```bash
az webapp config appsettings list \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --output table
```

### 8.4. Xóa một setting

```bash
az webapp config appsettings delete \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --setting-names <SettingName>
```

---

## Bước 9: Test CI/CD

### 9.1. Commit và push workflow

```bash
git add .github/workflows/ci-cd.yml
git add Dockerfile
git commit -m "Add CI/CD workflow"
git push origin main
```

### 9.2. Kiểm tra GitHub Actions

1. Vào repository → Tab **Actions**
2. Xem workflow đang chạy
3. Click vào workflow để xem chi tiết logs

### 9.3. Kiểm tra logs nếu có lỗi

**Build job failed:**

- Xem logs trong GitHub Actions
- Kiểm tra Dockerfile syntax
- Đảm bảo dependencies build được

**Deploy job failed:**

- Kiểm tra secrets đã đúng chưa
- Xem logs Azure App Service

### 9.4. Xem logs Azure App Service

```bash
# Real-time logs
az webapp log tail \
  --name <AppName> \
  --resource-group <ResourceGroupName>

# Download logs
az webapp log download \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --log-file app-logs.zip
```

### 9.5. Test app đã deploy

```bash
# Test HTTP endpoint
curl https://<AppName>.azurewebsites.net

# Hoặc mở browser
open https://<AppName>.azurewebsites.net
```

**Ví dụ:**

```bash
curl https://talkingtails.azurewebsites.net
```

---

## Troubleshooting

### Lỗi 1: ImagePullFailure

**Triệu chứng:**

```
Container pull image failed with reason: ImagePullFailure
```

**Nguyên nhân:**

- App Service không pull được image từ ACR
- Managed Identity chưa setup đúng

**Giải pháp:**

```bash
# Kiểm tra Managed Identity đã bật chưa
az webapp config show \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --query acrUseManagedIdentityCreds

# Nếu false, bật lại
az webapp config set \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --generic-configurations '{"acrUseManagedIdentityCreds": true}'

# Kiểm tra role assignment
az role assignment list \
  --assignee <PrincipalId> \
  --scope /subscriptions/<SubscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.ContainerRegistry/registries/<ACRName>
```

### Lỗi 2: Container starts then stops

**Triệu chứng:**

```
Container is running.
Site startup probe succeeded.
Container is terminating.
```

**Nguyên nhân:**

- App crash ngay sau khi start
- Port không đúng
- Startup command sai

**Giải pháp:**

**Kiểm tra port:**

```bash
# Đảm bảo app listen đúng port
az webapp config appsettings set \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --settings WEBSITES_PORT="8080"

# Trong Dockerfile:
# EXPOSE 8080
# ENV ASPNETCORE_URLS=http://+:8080
```

**Xem container logs:**

```bash
az webapp log tail --name <AppName> --resource-group <ResourceGroupName>
```

**Kiểm tra startup command:**

```bash
# Xem command hiện tại
az webapp config show \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --query appCommandLine

# Set lại (hoặc để trống để dùng ENTRYPOINT từ Dockerfile)
az webapp config set \
  --name <AppName> \
  --resource-group <ResourceGroupName> \
  --startup-file ""
```

### Lỗi 3: GitHub Actions - Login failed

**Triệu chứng:**

```
Error: Login failed with Error: Get Token request returned: 401
```

**Nguyên nhân:**

- `AZURE_CREDENTIALS` sai hoặc expired

**Giải pháp:**

```bash
# Tạo lại service principal
az ad sp create-for-rbac \
  --name "github-actions-<AppName>" \
  --role Contributor \
  --scopes /subscriptions/<SubscriptionId>/resourceGroups/<ResourceGroupName> \
  --sdk-auth

# Update lại GitHub Secret AZURE_CREDENTIALS
```

### Lỗi 4: ACR login failed

**Triệu chứng:**

```
Error: unauthorized: authentication required
```

**Nguyên nhân:**

- `ACR_USERNAME` hoặc `ACR_PASSWORD` sai

**Giải pháp:**

```bash
# Lấy lại credentials
az acr credential show \
  --name <ACRName> \
  --query username -o tsv

az acr credential show \
  --name <ACRName> \
  --query passwords[0].value -o tsv

# Update lại GitHub Secrets
```

### Lỗi 5: Cannot resolve host

**Triệu chứng:**

```bash
curl: (6) Could not resolve host: myapp.azurewebsites.net
```

**Nguyên nhân:**

- App đang stopped
- DNS chưa propagate

**Giải pháp:**

```bash
# Restart app
az webapp restart \
  --name <AppName> \
  --resource-group <ResourceGroupName>

# Đợi 1-2 phút rồi thử lại
sleep 60
curl https://<AppName>.azurewebsites.net
```

### Lỗi 6: Site is blocked

**Triệu chứng:**

```
Site: <AppName> will be blocked for X minutes
Start site prohibited because the site is being blocked
```

**Nguyên nhân:**

- Container failed nhiều lần liên tiếp
- Azure tạm khóa để tránh infinite restart loop

**Giải pháp:**

```bash
# Đợi hết thời gian block (1-5 phút)
# Trong lúc đợi, fix lỗi container (xem logs)

# Sau khi hết block, restart
az webapp restart \
  --name <AppName> \
  --resource-group <ResourceGroupName>
```

---

## Checklist tổng hợp

### ☑️ Chuẩn bị

- [ ] Có tài khoản Azure
- [ ] Có repository GitHub
- [ ] Code đã push lên GitHub
- [ ] Azure CLI hoặc Cloud Shell sẵn sàng

### ☑️ Azure Resources

- [ ] Tạo Resource Group
- [ ] Tạo Azure Container Registry (ACR)
- [ ] Enable admin user cho ACR
- [ ] Lấy ACR username/password
- [ ] Tạo App Service Plan
- [ ] Tạo App Service (Web App)

### ☑️ Security Setup

- [ ] Enable Managed Identity cho App Service
- [ ] Gán quyền AcrPull cho Managed Identity
- [ ] Bật `acrUseManagedIdentityCreds`
- [ ] Tạo Service Principal cho GitHub Actions
- [ ] Lưu JSON từ Service Principal

### ☑️ GitHub Setup

- [ ] Thêm secret `AZURE_CREDENTIALS`
- [ ] Thêm secret `ACR_USERNAME`
- [ ] Thêm secret `ACR_PASSWORD`

### ☑️ Code Setup

- [ ] Tạo Dockerfile
- [ ] Tạo `.github/workflows/ci-cd.yml`
- [ ] Config app settings trên Azure (không commit secrets)
- [ ] Commit và push code

### ☑️ Testing

- [ ] Workflow chạy thành công trên GitHub Actions
- [ ] Image được push lên ACR
- [ ] App được deploy lên Azure
- [ ] App accessible qua URL
- [ ] Logs không có lỗi

---

## Tài liệu tham khảo

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Container Registry](https://learn.microsoft.com/en-us/azure/container-registry/)
- [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/)
- [Managed Identities](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)

---

## Ghi chú quan trọng

### 🔒 Bảo mật

- **KHÔNG BAO GIỜ** commit secrets vào Git (passwords, connection strings, API keys, JWT secrets)
- **KHÔNG** lưu command chứa secrets trong file README hoặc docs
- Dùng Azure App Settings cho config/secrets
- Rotate Service Principal credentials định kỳ (ít nhất 90 ngày)
- Xóa Service Principal khi không dùng nữa
- Sử dụng `.gitignore` để bảo vệ:
  - `appsettings.Production.json`
  - `appsettings.Development.json` (nếu chứa secrets)
  - Các file `.env`

### 💰 Chi phí

- **GitHub Actions**: Miễn phí (2000 phút/tháng cho public repo)
- **ACR Basic**: ~$5/tháng
- **App Service B1**: ~$13/tháng
- **TỔNG**: ~$18/tháng
