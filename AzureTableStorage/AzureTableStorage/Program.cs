using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableStorage
{
    class Program
    {
        static CloudStorageAccount storageAccount;
        static CloudTableClient tableClient;
        static CloudTable table;
        
        
        static char[] partitionKey = { 'A', 'A', 'A' };
        static char[] rowKey = { '0', '0', '0' };

        static void Main(string[] args)
        {
            try
            {
                CreateAzureStorageTable();
                do
                {
                    Console.WriteLine("Enter the operation you want to perform");
                    Console.WriteLine("0.To Add\n1.To Retrieve\n2.To Update\n3.To Delete\n4.To Exit");
                    int oper = Convert.ToInt32(Console.ReadLine());
                    switch (oper)
                    {
                        case 0:
                            {
                                AddGuestIdentity();
                                break;
                            }
                        case 1:
                            {
                                RetrieveGuestIdentity();
                                break;
                            }
                        case 2:
                            {
                                UpdateGuestIdentity();
                                break;
                            }
                        case 3:
                            {
                                DeleteGuestIdentity();
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                while (true);
                
                
                //DeleteAzureStorageTable();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
            
        }
        private static void CreateAzureStorageTable()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            tableClient = storageAccount.CreateCloudTableClient();

            table = tableClient.GetTableReference("guests");

            table.CreateIfNotExists();
            Console.WriteLine("Table Created");
            
        }
        class GuestEntity : TableEntity
        {
            public string Name { get; set; }
            public string ContactNumber { get; set; }
            public GuestEntity() { }
            public GuestEntity(string partitionKey, string rowKey)
            {
                this.PartitionKey = partitionKey;
                this.RowKey = rowKey;
            }
        }
        private static void AddGuestIdentity()
        {

           
            string greetings = new string(partitionKey);
            string greetingsnew = new string(rowKey);
            GuestEntity guestEntity = new GuestEntity(greetings, greetingsnew);
            Console.WriteLine("Name: ");
            guestEntity.Name = Console.ReadLine();
            Console.WriteLine("Contact Number: ");
            guestEntity.ContactNumber = Console.ReadLine();
            TableOperation insertoperation = TableOperation.Insert(guestEntity);
            table.Execute(insertoperation);
            Console.WriteLine("Data Added");
            Console.WriteLine("your RowKey: ");
            
            rowKeyUpdate();
            if(rowKey[0]=='0'&&rowKey[1]=='0'&&rowKey[2]=='3')
            {
                rowKey[0] = '0';
                rowKey[1] = '0';
                rowKey[2] = '0';
                partitionKeyUpdate();
            }
            

        }

        private static void partitionKeyUpdate()
        {
            if (partitionKey[2] != 'Z')
                partitionKey[2] = Convert.ToChar(partitionKey[2] + 1);
            else
            {
                partitionKey[2] = 'A';

                if (partitionKey[1] != 'Z')
                    partitionKey[1] = Convert.ToChar(partitionKey[1] + 1);
                else
                {
                    partitionKey[1] = 'A';
                    partitionKey[0] = Convert.ToChar(partitionKey[0] + 1);
                }
            }

        }

        private static void rowKeyUpdate()
        {
            if (rowKey[2] != '9')
                rowKey[2] = Convert.ToChar(rowKey[2] + 1);
            else
            {
                rowKey[2] = '0';

                if (rowKey[1] != '9')
                    rowKey[1] = Convert.ToChar(rowKey[1] + 1);
                else
                {
                    rowKey[1] = '0';
                    rowKey[0] = Convert.ToChar(rowKey[0] + 1);
                }
            }
        }

        
    
        private static void RetrieveGuestIdentity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if (retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;
                Console.WriteLine($"Name:{guest.Name}  ContactNumber: {guest.ContactNumber}");
            }
            else
            {
                Console.WriteLine("Details Not Found");
            }
        }

        private static void UpdateGuestIdentity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if (retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;
                Console.WriteLine("Contact Number");
                guest.ContactNumber = Console.ReadLine();

                TableOperation updateOperation = TableOperation.Replace(guest);
                table.Execute(updateOperation);
                Console.WriteLine("Entity Updated");
            }
            else
            {
                Console.WriteLine("Details Not Updated");
            }
        }
        private static void DeleteGuestIdentity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if (retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;
                TableOperation deleteOperation = TableOperation.Delete(guest);
                table.Execute(deleteOperation);
                Console.WriteLine("Entity Deleted");
            }
            else
            {
                Console.WriteLine("Details could not be retrieved");
            }
        }
        private static void DeleteAzureStorageTable()
        {
            table.DeleteIfExists();
            Console.WriteLine("Table Deleted");
        }

    }

}
