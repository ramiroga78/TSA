using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.Utilities
{
    public class Regedit
    {
        private static string subKey = string.Empty;

        /// <summary>
        /// A property to set the SubKey value
        /// (default = "SOFTWARE\\" + Application.ProductName.ToUpper())
        /// </summary>
        public static string SubKey
        {
            get { return subKey; }
            set { subKey = value; }
        }

        private static RegistryKey baseRegistryKey = Registry.LocalMachine;

        /// <summary>
        /// A property to set the BaseRegistryKey value.
        /// (default = Registry.LocalMachine)
        /// </summary>
        public static RegistryKey BaseRegistryKey
        {
            get { return baseRegistryKey; }
            set { baseRegistryKey = value; }
        }

        /// <summary>
        /// To read a registry key.
        /// input: KeyName (string)
        /// output: value (string) 
        /// </summary>
        public static Response Read(string KeyName)
        {
            Response response = new Response();
            // Opening the registry key
            RegistryKey rk = baseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                response.Result = "ERROR";
                response.Message = "No se encontró la ruta especificada";

                return response;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    response.Result = "OK";
                    response.Message = (string)sk1.GetValue(KeyName);

                    return response;
                }
                catch (Exception e)
                {
                    response.Result = "ERROR";
                    response.Message = e.Message;

                    return response;
                }
            }
        }

        public static Response ReadAll()
        {
            Response response = new Response();
            List<Response.ResponseValues> rvalues = new List<Response.ResponseValues>();
            // Opening the registry key
            RegistryKey rk = baseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                response.Result = "ERROR";
                response.Message = "No se encontró la ruta especificada";

                return response;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    foreach (string valueName in sk1.GetValueNames())
                    {
                        Response.ResponseValues value = new Response.ResponseValues();

                        if (valueName == "")
                        {
                            value.Key = "(Default)";
                        }
                        else
                        {
                            value.Key = valueName;
                        }

                        value.Value = sk1.GetValue(valueName).ToString();

                        rvalues.Add(value);
                    }

                    response.Result = "OK";
                    response.Message = "Valores de " + sk1.Name.ToUpper();
                    response.Values = rvalues;

                    return response;
                }
                catch (Exception e)
                {
                    response.Result = "ERROR";
                    response.Message = e.Message;

                    return response;
                }
            }
        }



        public static string[] ReadAllStringFormat()
        {
            // Opening the registry key
            RegistryKey rk = baseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    return sk1.GetValueNames();
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    //ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                    return null;
                }
            }
        }

        /// <summary>
        /// To read a registry key.
        /// input: KeyName (string)
        /// output: value (string) 
        /// </summary>
        public static string ReadValue(string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = baseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    //ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                    return null;
                }
            }
        }

        /// <summary>
        /// To write into a registry key.
        /// input: KeyName (string) , Value (object)
        /// output: true or false 
        /// </summary>
        public static Response Write(string KeyName, object Value)
        {
            Response response = new Response();

            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                // I have to use CreateSubKey 
                // (create or open it if already exits), 
                // 'cause OpenSubKey open a subKey as read-only
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                // Save the value
                sk1.SetValue(KeyName.ToUpper(), Value);

                response.Result = "OK";
                response.Message = "Se escribió correctamente en " + KeyName.ToUpper();

            }
            catch (Exception e)
            {
                response.Result = "ERROR";
                response.Message = e.Message;
            }
            return response;
        }


        /// <summary>
        /// To delete a registry key.
        /// input: KeyName (string)
        /// output: true or false 
        /// </summary>
        public static Response DeleteKey(string KeyName)
        {
            Response response = new Response();

            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                // If the RegistrySubKey doesn't exists -> (true)
                sk1.DeleteValue(KeyName);

                response.Result = "OK";
                response.Message = "Se eliminó correctamente " + KeyName.ToUpper();
            }
            catch (Exception e)
            {
                response.Result = "ERROR";
                response.Message = e.Message;
            }
            return response;
        }

    }
}
