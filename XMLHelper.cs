using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XML_Helpers
{
    public class XMLHelper
    {
      
        // XML Parsing Methods
        public static ValidationResult ValidateXML(XmlDocument document, string schemaContent, string targetNameSpace = null)
        {
            bool isvalid = true;

            using (ValidationResult result = new ValidationResult())
            {
                try
                {
                    if (document != null && schemaContent != null)
                    {
                        XmlSchemaSet xmlSchema = new XmlSchemaSet();
                        xmlSchema.Add(targetNameSpace == null ? "" : targetNameSpace, XmlReader.Create(new StringReader(schemaContent)));
                        document.Schemas.Add(xmlSchema);
                        document.Validate((sender, e) =>
                        {
                            isvalid = false;

                            result.ValidationErrors.Add(new ValidationError()
                            {

                                Severity = e.Severity.ToString(),
                                Message = e.Message

                            });

                        });

                    }
                    else
                    {
                        throw new NullReferenceException("XML Document, XML Schema Path Must Not be Null.");
                    }

                }
                catch (Exception ex)
                {
                    isvalid = false;
                    result.ValidationErrors.Add(new ValidationError()
                    {
                        Message = ex.Message,
                        Severity = "Error"
                    });
                    Console.WriteLine(ex.Message);
                }

                result.IsValid = isvalid;

                return result;
            }
        }
        public static ValidationResult ValidateXML(XDocument document, string schemaContent, string targetNameSpace = null)
        {
            bool isvalid = true;
            string transactionID = string.Empty;
            using (ValidationResult result = new ValidationResult())
            {
                try
                {
                    if (document != null && schemaContent != null)
                    {
                        XmlSchemaSet xmlSchema = new XmlSchemaSet();
                        xmlSchema.Add(targetNameSpace == null ? "" : targetNameSpace, XmlReader.Create(new StringReader(schemaContent)));

                        document.Validate(xmlSchema, (sender, e) =>
                        {
                            isvalid = false;
                            result.ValidationErrors.Add(new ValidationError()
                            {
                                Severity = e.Severity.ToString(),
                                Message = e.Message
                            });
                        });
                    }
                    else
                    {
                        throw new NullReferenceException("XML Document, XML Schema Content Must Not be Null.");
                    }
                }
                catch (Exception ex)
                {
                    isvalid = false;
                    Console.WriteLine(ex);
                    result.ValidationErrors.Add(new ValidationError()
                    {
                        Message = ex.Message,
                        Severity = "Error"
                    });
                }

                result.IsValid = isvalid;

                return result;
            }

        }
        
        public static XmlDocument LoadXmlDocument(string path)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(path);
                return document;
            }
            catch (FileLoadException)
            {
                throw new FileLoadException("Unable to load file. Please check given file is in well define xml format.");
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("File not found for given path.");
            }
            catch
            {
                return null;
            }
            finally
            {
                document = null;
            }
        }
        public static XDocument LoadXDocument(string path)
        {
            XDocument document;
            try
            {
                document = XDocument.Load(path);
                return document;
            }
            catch (FileLoadException)
            {
                throw new FileLoadException("Unable to load file. Please check given file is in well define xml format.");
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("File not found for given path.");
            }
            catch
            {
                return null;
            }
            finally
            {
                document = null;
            }
        }
    }
    public class ValidationResult : IDisposable
    {
        public ValidationResult()
        {
            ValidationErrors = new List<ValidationError>();
        }
        private bool disposedValue;

        public bool IsValid { get; set; }
        public string RequestType { get; set; }
        public string TargetNameSpace { get; set; }
        public string Code { get; set; }
        public string CorrelationId { get; set; }
        public string HttpCode { get; set; }
        public string Target { get; set; }
        public string ResponseType { get; set; }
        public List<ValidationError> ValidationErrors { get; set; }
        public XmlDocument XmlObject { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    public class ValidationError
    {
        public string Severity { get; set; }
        public string Message { get; set; }

    }
}
