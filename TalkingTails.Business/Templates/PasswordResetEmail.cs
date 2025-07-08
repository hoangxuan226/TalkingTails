namespace TalkingTails.Business.Templates
{
    public static class PasswordResetEmail
    {
        public static string GetTemplate(string userName, string resetLink)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Đặt lại mật khẩu - TalkingTails</title>
                    <style>
                        * {{
                            margin: 0;
                            padding: 0;
                            box-sizing: border-box;
                        }}
                        
                        body {{
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            background-color: #f8f9fa;
                        }}
                        
                        .email-container {{
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            border-radius: 10px;
                            overflow: hidden;
                            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
                        }}
                        
                        .header {{
                            background: linear-gradient(135deg, #D87C3A 0%, #E89B5A 100%);
                            padding: 40px 30px;
                            text-align: center;
                            color: white;
                        }}
                        
                        .logo {{
                            font-size: 28px;
                            font-weight: bold;
                            margin-bottom: 10px;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            gap: 10px;
                        }}
                        
                        .paw-icon {{
                            font-size: 24px;
                            color: #fff;
                        }}
                        
                        .tagline {{
                            font-size: 14px;
                            opacity: 0.9;
                            margin-top: 5px;
                        }}
                        
                        .content {{
                            padding: 40px 30px;
                        }}
                        
                        .greeting {{
                            font-size: 24px;
                            color: #D87C3A;
                            margin-bottom: 20px;
                            font-weight: 600;
                        }}
                        
                        .message {{
                            font-size: 16px;
                            line-height: 1.8;
                            color: #555;
                            margin-bottom: 30px;
                        }}
                        
                        .cta-container {{
                            text-align: center;
                            margin: 40px 0;
                        }}
                        
                        .cta-button {{
                            display: inline-block;
                            background: linear-gradient(135deg, #D87C3A 0%, #E89B5A 100%);
                            color: white !important;
                            padding: 16px 32px;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: 600;
                            font-size: 16px;
                            transition: all 0.3s ease;
                            box-shadow: 0 4px 15px rgba(216, 124, 58, 0.3);
                        }}
                        
                        .cta-button:hover {{
                            transform: translateY(-2px);
                            box-shadow: 0 6px 20px rgba(216, 124, 58, 0.4);
                        }}
                        
                        .expiry-notice {{
                            background-color: #fff3cd;
                            border: 1px solid #ffeaa7;
                            border-radius: 8px;
                            padding: 15px;
                            margin: 25px 0;
                            font-size: 14px;
                            color: #856404;
                        }}
                        
                        .security-note {{
                            background-color: #f8f9fa;
                            border-left: 4px solid #D87C3A;
                            padding: 15px;
                            margin: 25px 0;
                            font-size: 14px;
                            color: #666;
                        }}
                        
                        .footer {{
                            background-color: #f8f9fa;
                            padding: 30px;
                            text-align: center;
                            border-top: 1px solid #e9ecef;
                        }}
                        
                        .signature {{
                            font-size: 16px;
                            color: #555;
                            margin-bottom: 20px;
                        }}
                        
                        .contact-info {{
                            font-size: 14px;
                            color: #999;
                            margin-bottom: 15px;
                        }}
                        
                        .contact-info a {{
                            color: #D87C3A;
                            text-decoration: none;
                        }}
                        
                        .social-links {{
                            margin-top: 20px;
                        }}
                        
                        .social-links a {{
                            display: inline-block;
                            margin: 0 10px;
                            color: #D87C3A;
                            font-size: 20px;
                            text-decoration: none;
                        }}
                        
                        .divider {{
                            height: 2px;
                            background: linear-gradient(to right, transparent, #D87C3A, transparent);
                            margin: 30px 0;
                        }}
                        
                        /* Responsive design */
                        @media (max-width: 600px) {{
                            .email-container {{
                                margin: 0 10px;
                            }}
                            
                            .header, .content, .footer {{
                                padding: 20px;
                            }}
                            
                            .greeting {{
                                font-size: 20px;
                            }}
                            
                            .cta-button {{
                                padding: 14px 28px;
                                font-size: 14px;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class=""email-container"">

                        
                        <div class=""content"">
                            <div class=""greeting"">Xin chào {userName},</div>
                            
                            <div class=""message"">
                                Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản TalkingTails của bạn. 
                                Đừng lo lắng, việc này hoàn toàn an toàn và dễ dàng!
                            </div>
                            
                            <div class=""cta-container"">
                                <a href=""{resetLink}"" class=""cta-button"">
                                    🔒 Đặt lại mật khẩu
                                </a>
                            </div>
                            
                            <div class=""expiry-notice"">
                                <strong>Lưu ý:</strong> Liên kết này sẽ hết hạn sau <strong>1 giờ</strong> để đảm bảo an toàn cho tài khoản của bạn.
                            </div>
                            
                            <div class=""security-note"">
                                <strong>Bảo mật:</strong> Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này. 
                                Tài khoản của bạn vẫn hoàn toàn an toàn.
                            </div>
                        </div>
                        
                        <div class=""footer"">
                            <div class=""signature"">
                                <strong>Trân trọng,</strong><br>
                                Đội ngũ TalkingTails 🐕🐱
                            </div>

                            <!--
                            <div class=""contact-info"">
                                📧 <a href=""mailto:talkingtails124@gmail.com"">talkingtails124@gmail.com</a><br>
                                🌐 <a href=""https://talkingtails.com"">talkingtails.com</a>
                            </div>            

                            <div class=""social-links"">
                                <a href=""#"" title=""Facebook"">📘</a>
                                <a href=""#"" title=""Instagram"">📷</a>
                                <a href=""#"" title=""Twitter"">🐦</a>
                            </div>
                            -->
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}