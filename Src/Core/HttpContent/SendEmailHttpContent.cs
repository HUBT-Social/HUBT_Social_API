namespace HUBT_Social_API.Src.Core.HttpContent;

public static class SendEmailHttpContent
{
    private const string Style =
        @"<style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }
                .email-container {
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                    text-align: center;
                }
                .header {
                    font-size: 24px;
                    font-weight: bold;
                    margin-bottom: 20px;
                    color: #333333;
                }
                .code-box {
                    display: inline-block;
                    font-size: 36px;
                    font-weight: bold;
                    color: #333333;
                    border: 1px solid #dddddd;
                    border-radius: 8px;
                    padding: 10px 20px;
                    background-color: #f9f9f9;
                    margin: 20px 0;
                }
                .footer {
                    font-size: 14px;
                    color: #666666;
                    margin-top: 20px;
                }
                .footer a {
                    color: #007bff;
                    text-decoration: none;
                }
                .footer a:hover {
                    text-decoration: underline;
                }
            </style>";

    public static string GetSendPostcodeContent(string postcode)
    {
        return $@"<!DOCTYPE html>
<html>
<head>
    {Style}
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">Logo</div>
        <p>Here is your login approval code:</p>
        <div class=""code-box"">{postcode}</div>
        <div class=""footer"">
            <p>If this request did not come from you, change your account password immediately to prevent further unauthorized access. Read <a href=""#"">Protecting Your Account</a> for tips on password strength.</p>
        </div>
    </div>
</body>
</html>";
    }
}