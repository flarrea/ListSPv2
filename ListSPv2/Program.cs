using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace ListSPv2
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName = "Usuario de Sharepoint Online"; 
            string password = "Password de Usuario";
            SecureString securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            using (var clientContext = new ClientContext("https://tecnofranchile.sharepoint.com/sites/Demo/")) //Dirección del sitio en Sharepoint Online
            {
                // Credenciales para acceder a Sharepoint Online    
                clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassword);
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                List productList = web.Lists.GetByTitle("Clientes"); //Nombre de la lista creada en el sitio
                DataTable dt = new DataTable();
                dt = GetDatafromSQL();
                foreach (DataRow dr in dt.Rows) // Recorrido por la lista.  
                {
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem newItem = productList.AddItem(itemCreateInfo);
                    //Campo de la tabla y campo de la lista
                    newItem["ClienteID"] = dr["ID"];
                    newItem["Nombre"] = dr["Nombre"];
                    newItem["Apellido"] = dr["Apellido"];
                    newItem["Telefono"] = dr["Telefono"];
                    newItem["Ocupacion"] = dr["Ocupacion"];
                    newItem.Update();
                    clientContext.Load(newItem);
                    clientContext.ExecuteQuery();

                }
                clientContext.Load(productList);
                clientContext.ExecuteQuery();

            }
        }

        private static DataTable GetDatafromSQL()
        {
            DataTable dataTable = new DataTable();
            //string connString = @"Server=<URL del Servidor>;Database=<Nombre de la DB>;Integrated Security=True";
            //string query = "Consulta a SQL";
            //Ejemplo
            string connString = @"Server=(localdb)\MSSQLLocalDB;Database=Practica_Patrones;Integrated Security=True";
            string query = "SELECT c.ID, c.Nombre, c.Apellido, c.Telefono, c.Ocupacion from dbo.Clientes c where c.ID<100;";

            SqlConnection connection = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, connection);
            connection.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dataTable);
            connection.Close();
            da.Dispose();

            return dataTable;
        }
    }
}
