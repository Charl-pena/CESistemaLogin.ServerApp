using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace TBAnalisisFinanciero.Client.Auth;

public class PDFBaseLogic : ComponentBase
{
   protected const long maxFileSize = 1024 * 2000;
   protected const int maxAllowedFiles = 1;
   
   protected static bool IsValidFile(IBrowserFile file, out string validationError)
   {
      validationError = string.Empty;

      if (file.ContentType != "application/pdf" || !file.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
      {
         validationError = "Por favor, sube un archivo ePub válido.";
         return false;
      }

      if (file.Size > maxFileSize)
      {
         validationError = "El archivo es demasiado grande. El tamaño máximo permitido es de 2 MB.";
         return false;
      }

      return true;
   }
}