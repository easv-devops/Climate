using api.helpers;
using infrastructure.Models;

namespace service.services.notificationServices;

public class EmailBuilder
{ 
    public string BuildWelcomeMessage(string userName) 
    { 
        //todo load from program so it know if it is localhost or server ip..
        string siteUrl = "http://localhost:4200/";
        string imagePath = "backend/assets/logo_light_no_bg.png";
        
        string htmlMessage = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Velkommen til Climate Platform</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    padding: 20px;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #fff;
                    padding: 20px;
                    border-radius: 10px;
                    box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                }}
                h1 {{
                    color: #4CAF50;
                }}
                p {{
                    margin-bottom: 20px;
                }}
                img {{
                    display: block;
                    margin: 0 auto;
                    max-width: 100%;
                    height: auto;
                }}
                a {{
                    color: #4CAF50;
                    text-decoration: none;
                }}
                a:hover {{
                    text-decoration: underline;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Velkommen til Climate Platform, {userName}!</h1>
                <p>Tak fordi du valgte Climate Platform til at monitorere indendørs klima. Vi er her for at hjælpe dig med at skabe et sundt og behageligt miljø.</p>
                <img src='data:image/jpeg;base64,{ConvertPngToBase64(imagePath)}' alt='Climate Platform' />
                <p>For mere information, besøg vores hjemmeside: <a href='{siteUrl}'>{siteUrl}</a></p>
            </div>
        </body>
        </html>
    ";

    return htmlMessage;
}
    private string ConvertPngToBase64(string imagePath)
    {
        byte[] imageData = File.ReadAllBytes(imagePath);
        return Convert.ToBase64String(imageData);
    }
}