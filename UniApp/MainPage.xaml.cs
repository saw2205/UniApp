using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        async private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Progress.IsActive = true;
           
            // Create the table client.
            CloudTableClient tableClient = App.storageAccount.CreateCloudTableClient();
            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("StudentLogin");

            // Create a new customer entity.
            //StudentEntity student1 = new StudentEntity("666", "666");
            ////customer1.Email = "Walter@contoso.com";
            ////customer1.PhoneNumber = "425-555-0101";
            //student1.PassKey = "666";
            //// Create the table query.
            //TableQuery<StudentEntity> rangeQuery = new Microsoft.WindowsAzure.Storage.Table.TableQuery<StudentEntity>().Where(
            //    TableQuery.CombineFilters(
            //        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, UserNameBox.Text),
            //        TableOperators.And,
            //        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, PassWordBox.Password)));

            TableOperation retrievePassKeyOperation = TableOperation.Retrieve<StudentEntity>(UserNameBox.Text, PassWordBox.Password);
            TableResult retrievedResult = await table.ExecuteAsync(retrievePassKeyOperation);


           // // Create the TableOperation that inserts the customer entity.
           // TableOperation insertOperation = TableOperation.Insert(student1);

           // // Execute the insert operation.
           //await table.ExecuteAsync(insertOperation);
           Progress.IsActive = false;

          // TableContinuationToken t = new Microsoft.WindowsAzure.Storage.Table.TableContinuationToken();
            
            // Loop through the results, displaying information about the entity.
           if (retrievedResult.Result != null)
           {
               
               //MessageDialog MD = new MessageDialog(((StudentEntity)retrievedResult.Result).PassKey,"PassKey is" );
               //await MD.ShowAsync();
               NavigationParameters NPara = new NavigationParameters();
               NPara.PKey=((StudentEntity)retrievedResult.Result).PassKey;
               NPara.SName = ((StudentEntity)retrievedResult.Result).StudentName;
               Frame.Navigate(typeof(CourseRegistrationPage),NPara);

           }
           else
           {
               
               MessageDialog MD = new MessageDialog("Not found !!!", "Credentials ");
               await MD.ShowAsync();
 
           }
                
                //Console.WriteLine("PassKey is ---> {0}", entity.PassKey);
            
           

        }
    }
}
