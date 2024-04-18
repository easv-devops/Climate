using api.helpers;
using infrastructure.Models;

namespace service.services.notificationServices;

public class EmailBuilder
{ 
    public string BuildWelcomeMessage(string userName) 
    { 
        //todo load from program so it know if it is localhost or server ip..
        string siteUrl = "http://localhost:4200/";
        string template = 
            $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Welcome to Climate</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f2f5f8;
            margin: 0;
            padding: 0;
        }}

        .container {{
            max-width: 600px;
            margin: 50px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            text-align: center;
        }}

        h1 {{
            color: #007bff;
        }}

        p {{
            color: #555555;
            font-size: 16px;
            line-height: 1.6;
            text-align: center;
            margin-bottom: 20px;
        }}
        a {{
                color: #007bff;
                text-decoration: none;
                font-weight: bold; /* Making the text bold */
            }}
        .graph {{
            width: 100%;
            margin: 20px auto;
            border-radius: 10px;
            overflow: hidden;
        }}

        .graph img {{
            width: 200px;
            display: block;
            margin: 0 auto;
        }}

        p:last-child {{
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Welcome to Climate</h1>
        <p>Dear {userName},</p>
        <p>We are thrilled to welcome you aboard Climate, your ultimate solution for preventing mold, improving indoor climate, and gathering valuable data.</p>
        <p>With Climate, you can effortlessly monitor and optimize your indoor environment to ensure a healthier and more comfortable living space.</p>
        <div class=""graph"">
            <!-- Placeholder for animated graph (could be an actual graph image) -->
            <img src=""https://www.shutterstock.com/image-vector/water-wave-logo-template-vector-260nw-1200752182.jpg"" alt=""Animated Graph"">
        </div>
        <p>Get started today and take control of your indoor climate with Climate!</p>
        <a href='{siteUrl}'>Go To Climate Now</a>
        <p>Best regards,<br>The Climate Team</p>
    </div>
</body>
</html>
";
        return template; 
    }


  public string BuildResetPasswordMessage(string newPassword)
{
    string template =
        $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f2f5f8;
            margin: 0;
            padding: 0;
        }}

        .container {{
            max-width: 600px;
            margin: 50px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            text-align: center;
        }}

        h1 {{
            color: #007bff;
        }}

        p {{
            color: #555555;
            font-size: 16px;
            line-height: 1.6;
            text-align: center;
            margin-bottom: 20px;
        }}

        .password-box {{
            background-color: #f2f2f2;
            border: 1px solid #ccc;
            border-radius: 5px;
            padding: 10px;
            margin-bottom: 20px;
        }}

        .password-box p {{
            margin: 0;
        }}

        .password-text {{
            font-size: 24px;
            font-weight: bold;
            color: #007bff;
            margin-bottom: 10px;
        }}

        .graph {{
            width: 100%;
            margin: 20px auto;
            border-radius: 10px;
            overflow: hidden;
        }}

        .graph img {{
            width: 200px;
            display: block;
            margin: 0 auto;
        }}

        p:last-child {{
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Your Password Has Been Reset</h1>
        <p>Your password has been successfully reset. You can now use your new password to access your account.</p>
        <div class=""password-box"">
            <p class=""password-text"">Your New Password:</p>
            <p>{newPassword}</p>
        </div>
        <p>If you did not reset your password, please contact us.</p>
        <div class=""graph"">
            <img src=""https://www.shutterstock.com/image-vector/water-wave-logo-template-vector-260nw-1200752182.jpg"" alt=""Animated Graph"">
        </div>
        <p>Thank you for choosing our service.</p>
        <p>Best regards,<br>The Climate Team</p>
    </div>
</body>
</html>
";
    return template;
}
}