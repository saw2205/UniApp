using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CourseRegistrationPage : Page
    {
        public string PsKey;
        public string RetrievedRKey;
        public Dictionary<String, String> ListOfCoursesKeyValuePair;
        public string CourseName;
        string CourseCode;
        public string SubstituteCourseCode;
        public CourseRegistrationPage()
        {
            this.InitializeComponent();
            this.Loaded += CourseRegistrationPage_Loaded;
        }

        async void CourseRegistrationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // CheckProgress.IsActive = true;
            AvailableCourseListProgress.IsIndeterminate = true;
            CloudTableClient tableClient = App.storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("CourseListFinal");
            TableQuery<Course> query = new TableQuery<Course>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "1"));
            TableContinuationToken tct = null;
            var result = await table.ExecuteQuerySegmentedAsync<Course>(query, tct);
            ListOfCoursesKeyValuePair = new Dictionary<string, string>();
            foreach (var x in result)
            {
                ListOfCoursesKeyValuePair.Add(x.Field1, x.Field2);
            }
            var res = result.Results;
            CourseAvailableList.ItemsSource = res;
            //CheckProgress.IsActive = false;
            AvailableCourseListProgress.IsIndeterminate = false;
            //throw new NotImplementedException();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //MessageDialog ParameterPassed = new MessageDialog("Hello " +(e.Parameter as NavigationParameters).SName);
            //await ParameterPassed.ShowAsync();
            HelloTextBlock.Text = "Hello " + (e.Parameter as NavigationParameters).SName + ",";
            PsKey = (e.Parameter as NavigationParameters).PKey;
            RefreshRegCourseList();
            //Task DisplayRegisteredCourse = new Task(DisplayRCourseList);
            //CourseListRegLoadProgress.IsIndeterminate = true;
            //DisplayRegisteredCourse.Start();
            base.OnNavigatedTo(e);
        }

        private void RefreshRegCourseList()
        {
            TaskStatusBox.Text = string.Empty;
            Task DisplayRegisteredCourse = new Task(DisplayRCourseList);
            CourseListRegLoadProgress.IsIndeterminate = true;
            DisplayRegisteredCourse.Start();
        }

        async private void DisplayRCourseList()
        {
            CloudTableClient tableClient = App.storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("StudentCourseReg");
            TableQuery<Course> query = new TableQuery<Course>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PsKey));
            TableContinuationToken tct = null;
            var result = await table.ExecuteQuerySegmentedAsync<Course>(query, tct);


            var res = result.Results;
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RegisteredCourseList.ItemsSource = res;
                CourseListRegLoadProgress.IsIndeterminate = false;
            });

        }

        private void CourseAvailableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // StackPanel CheckTagPanel = sender as StackPanel;
            // MessageDialog randomdialog = new MessageDialog(CheckTagPanel.Tag.ToString());
            //await randomdialog.ShowAsync();
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SubstituteCourseCode = (sender as StackPanel).Tag.ToString();
            //StackPanel CheckTagPanel = sender as StackPanel;
            //MessageDialog randomdialog = new MessageDialog(CheckTagPanel.Tag.ToString());
            //await randomdialog.ShowAsync();
        }

        async private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RetrievedRKey = (sender as Image).Tag.ToString();
            MessageDialog RemoveDialodBox = new MessageDialog("Do you want to remove this course?");
            RemoveDialodBox.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(CommandHandler)));
            RemoveDialodBox.Commands.Add(new UICommand("No", new UICommandInvokedHandler(CommandHandler)));
            RemoveDialodBox.DefaultCommandIndex = 1;
            RemoveDialodBox.DefaultCommandIndex = 2;
            await RemoveDialodBox.ShowAsync();
        }

        async private void CommandHandler(IUICommand command)
        {
            var ComLabel = command.Label;
            switch (ComLabel)
            {
                case "Yes":
                    CloudTableClient tableClient = App.storageAccount.CreateCloudTableClient();

                    //Create the CloudTable that represents the "people" table.
                    CloudTable table = tableClient.GetTableReference("StudentCourseReg");

                    // Create a retrieve operation that expects a customer entity.
                    TableOperation retrieveOperation = TableOperation.Retrieve<Course>(PsKey, RetrievedRKey);

                    // Execute the operation.
                    TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

                    // Assign the result to a CustomerEntity.
                    Course deleteEntity = (Course)retrievedResult.Result;

                    // Create the Delete TableOperation.
                    if (deleteEntity != null)
                    {
                        TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                        TaskStatusBox.Text = "Please Wait.Removing Course.....";
                        TaskStatusRing.IsActive = true;
                        // Execute the operation.
                        await table.ExecuteAsync(deleteOperation);
                        TaskStatusRing.IsActive = false;
                        TaskStatusBox.Text = "Course removed.";
                        RefreshRegCourseList();
                        //HelloTextBlock.Text = "deleted";
                    }

                    else
                    { //HelloTextBlock.Text = "Could not be deleted";
                        TaskStatusBox.Text = "Couldn't remove course.Try Again.";
                    }


                    break;
                case "No":

                    break;
            }

        }

        async private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            RetrievedRKey = (sender as Image).Tag.ToString();
            CourseCode = (sender as Image).Tag.ToString();
            MessageDialog SubstituteDialogBox = new MessageDialog("Do you want to substitute " + CourseCode + " with " + SubstituteCourseCode + "?");
            SubstituteDialogBox.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(CommandHandlerSubstitute)));
            SubstituteDialogBox.Commands.Add(new UICommand("No", new UICommandInvokedHandler(CommandHandlerSubstitute)));


            await SubstituteDialogBox.ShowAsync();

        }

      async private void CommandHandlerSubstitute(IUICommand command)
        {
            var ComLabelSubstitute = command.Label;
            switch (ComLabelSubstitute)
            {
                case "Yes":
                    TaskStatusBox.Text = "Please Wait.Updating.....";
                    TaskStatusRing.IsActive = true;
                    CloudTableClient tableClient = App.storageAccount.CreateCloudTableClient();

                    // Create the CloudTable object that represents the "people" table.
                    CloudTable table = tableClient.GetTableReference("StudentCourseReg");

                    // Create a retrieve operation that takes a customer entity.
                    TableOperation retrieveOperation = TableOperation.Retrieve<Course>(PsKey,RetrievedRKey);

                    // Execute the operation.
                    TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

                    // Assign the result to a CustomerEntity object.
                    Course updateEntity = (Course)retrievedResult.Result;

                    if (updateEntity != null)
                    {
                        // Change the phone number.
                        //updateEntity.PartitionKey = PsKey;
                        //updateEntity.RowKey = SubstituteCourseCode;
                        updateEntity.Field1 = SubstituteCourseCode;
                        string RCourseName;
                        if (ListOfCoursesKeyValuePair.TryGetValue(SubstituteCourseCode, out RCourseName))
                        {
                            updateEntity.Field2 = RCourseName;
                        }


                        // Create the InsertOrReplace TableOperation
                        TableOperation updateOperation = TableOperation.Replace(updateEntity);

                        // Execute the operation.
                        await table.ExecuteAsync(updateOperation);
                        TaskStatusBox.Text = "Course Substituted";
                        TaskStatusRing.IsActive = false;
                        RefreshRegCourseList();

                        //Console.WriteLine("Entity updated.");
                    }

                    else
                    {
                        TaskStatusBox.Text = "Can't Update.";
                    }
                        //Console.WriteLine("Entity could not be retrieved.");

                    break;
                case "No": break;
            }
        }

        async private void Image_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            //            TaskStatusRing.IsActive = true;
            CourseCode = (sender as Image).Tag.ToString();
            string CourseN;
            if (ListOfCoursesKeyValuePair.TryGetValue(CourseCode, out CourseN))
            {
                CourseName = CourseN;
                MessageDialog DoYouWantToAddThisCourse = new MessageDialog("Do you want to add " + CourseCode + " " + CourseName + "?");
                DoYouWantToAddThisCourse.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(CommandHandlerAdd)));

                DoYouWantToAddThisCourse.Commands.Add(new UICommand("No", new UICommandInvokedHandler(CommandHandlerAdd)));
                DoYouWantToAddThisCourse.DefaultCommandIndex = 1;
                DoYouWantToAddThisCourse.CancelCommandIndex = 2;
                await DoYouWantToAddThisCourse.ShowAsync();

            }
            else
            {
                CourseName = string.Empty;
            }
            e.Handled = true;
            //            // Retrieve the storage account from the connection string.


            //// Create the table client.
            //CloudTableClient tableClient =App.storageAccount.CreateCloudTableClient();

            //// Create the CloudTable object that represents the "people" table.
            //CloudTable table = tableClient.GetTableReference("CourseListFinal");

            //// Create a retrieve operation that takes a customer entity.
            //TableOperation retrieveOperation = TableOperation.Retrieve<Course>("1", CourseCode);

            //// Execute the retrieve operation.
            //TableResult retrievedResult =await table.ExecuteAsync(retrieveOperation);

            //// Print the phone number of the result.
            //if (retrievedResult.Result != null)
            //{
            //    MessageDialog StupidityDialogBox = new MessageDialog(((Course)retrievedResult.Result).Field2);
            //  await  StupidityDialogBox.ShowAsync();
            //  TaskStatusRing.IsActive = false;
            //}
            //else
            //{
            //    TaskStatusBox.Text = "Could not fecth the data.";
            //} 
        }

        async private void CommandHandlerAdd(IUICommand command)
        {
            var ComLabelAdd = command.Label;

            switch (ComLabelAdd)
            {

                case "Yes":
                    TaskStatusRing.IsActive = true;
                    TaskStatusBox.Text = "Please Wait.Adding Course.";
                    // Create the table client.
                    CloudTableClient tableClient = App.storageAccount.CreateCloudTableClient();

                    // Create the CloudTable object that represents the "people" table.
                    CloudTable table = tableClient.GetTableReference("StudentCourseReg");

                    // Create a new customer entity.
                    Course StudentCourse = new Course(PsKey, CourseCode);
                    StudentCourse.Field1 = CourseCode;
                    StudentCourse.Field2 = CourseName;

                    // Create the TableOperation that inserts the customer entity.
                    TableOperation insertOperation = TableOperation.Insert(StudentCourse);

                    // Execute the insert operation.
                    await table.ExecuteAsync(insertOperation);
                    TaskStatusRing.IsActive = false;
                    TaskStatusBox.Text = "Course Added.";
                    RefreshRegCourseList();
                    break;
                case "No":
                    break;
            }

        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
