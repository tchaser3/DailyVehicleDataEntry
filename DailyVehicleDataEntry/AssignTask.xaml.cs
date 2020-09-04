/* Title:           Assign Task
 * Date:            12-27-19
 * Author:          Terry Holmes
 * 
 * Description:     This is used to for assigning a task */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NewEventLogDLL;
using DataValidationDLL;
using AssignedTasksDLL;
using NewEmployeeDLL;

namespace DailyVehicleDataEntry
{
    /// <summary>
    /// Interaction logic for AssignTask.xaml
    /// </summary>
    public partial class AssignTask : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValiationClass = new DataValidationClass();
        AssignedTaskClass TheAssignTaskClass = new AssignedTaskClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        AssignEmployeesDataSet TheAssignEmployeesDataSet = new AssignEmployeesDataSet();
        FindAssignedTasksByDateMatchDataSet TheFindAssignedTaskByDateMatchDataSet = new FindAssignedTasksByDateMatchDataSet();
        FindEmployeeEmailAddressDataSet TheFindEmployeeEmailAddressDataSet = new FindEmployeeEmailAddressDataSet();

        public AssignTask()
        {
            InitializeComponent();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void expClose_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            txtMessage.Text = "";
            txtSubject.Text = "";

            TheAssignEmployeesDataSet.assignemployees.Rows.Clear();
            TheComboEmployeeDataSet.employees.Rows.Clear();
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheComboEmployeeDataSet.employees.Rows.Clear();
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }
                    }
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Assign Task // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strFirstName;
            string strLastName;
            string strEmail;
            int intEmployeeID;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    intEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                    strFirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName;
                    strLastName = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName;

                    TheFindEmployeeEmailAddressDataSet = TheEmployeeClass.FindEmployeeEmailAddress(intEmployeeID);

                    if(TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].IsEmailAddressNull() == true)
                    {
                        strEmail = "NO EMAIL ADDRESS";
                        TheMessagesClass.InformationMessage(TheComboEmployeeDataSet.employees[intSelectedIndex].FullName + " Doesn't Have an Email Address");
                    }
                    else
                    {
                        strEmail = TheFindEmployeeEmailAddressDataSet.FindEmployeeEmailAddress[0].EmailAddress;
                    }


                    AssignEmployeesDataSet.assignemployeesRow NewEmployeeRow = TheAssignEmployeesDataSet.assignemployees.NewassignemployeesRow();

                    NewEmployeeRow.EmailAddress = strEmail;
                    NewEmployeeRow.EmployeeID = intEmployeeID;
                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastName;
                    NewEmployeeRow.SendEmail = true;

                    TheAssignEmployeesDataSet.assignemployees.Rows.Add(NewEmployeeRow);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Inspection // Assign Task // Select Employee Combobox " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expAssignTask_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            string strMessageSubject = "A Task Has Been Assigned For You - ";
            string strSubject = "";
            string strMessageText;
            string strErrorMessage = "";
            int intAssignedEmployeeID;
            int intOriginatingEmployeeID;
            bool blnEmailSuccess;
            string strServerMessage = "";
            DateTime datTransactionDate = DateTime.Now;
            int intTaskID;

            try
            {
                if (txtSubject.Text == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Subject Was Not Entered\n";
                }
                else
                {
                    strMessageSubject += txtSubject.Text + " - Do Not Reply";
                    strSubject = txtSubject.Text;
                }
                strServerMessage = txtMessage.Text;
                if (strServerMessage == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Message is Blank\n";
                }

                strMessageText = "<h3>" + strMessageSubject + "</h3>" + "<p>" + strServerMessage + "</p>";

                intNumberOfRecords = TheAssignEmployeesDataSet.assignemployees.Rows.Count - 1;

                intOriginatingEmployeeID = MainWindow.gintWarehouseEmployeeID;

                blnFatalError = TheAssignTaskClass.InsertAssignedTask(intOriginatingEmployeeID, datTransactionDate, strSubject, strServerMessage);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindAssignedTaskByDateMatchDataSet = TheAssignTaskClass.FindAssignedTaskByDateMatch(datTransactionDate);

                intTaskID = TheFindAssignedTaskByDateMatchDataSet.FindAssignedTasksByDateMatch[0].TransactionID;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intAssignedEmployeeID = TheAssignEmployeesDataSet.assignemployees[intCounter].EmployeeID;

                    blnFatalError = TheAssignTaskClass.InsertAssignedEmployeeTasks(intAssignedEmployeeID, intTaskID);

                    if (blnFatalError == true)
                        throw new Exception();

                    if (TheAssignEmployeesDataSet.assignemployees[intCounter].SendEmail == true)
                    {
                        blnEmailSuccess = TheSendEmailClass.SendEmail(TheAssignEmployeesDataSet.assignemployees[intCounter].EmailAddress, strMessageSubject, strMessageText);

                        if (blnEmailSuccess == false)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Tasks Have Been Assigned");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data entry // Assign Tasks // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
