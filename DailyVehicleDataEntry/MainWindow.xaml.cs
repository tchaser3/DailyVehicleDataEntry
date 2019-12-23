/* Title:       Daily Vehicle Data Entry
 * Date:        12-18-19
 * Author:      Terry Holmes
 * 
 * Description: This is the warehouse vehicle data entry window */

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DailyTrailerInspectionDLL;
using DataValidationDLL;
using DateSearchDLL;
using InspectionsDLL;
using TrailersDLL;
using VehicleAssignmentDLL;
using VehicleMainDLL;
using NewEmployeeDLL;
using NewEventLogDLL;

namespace DailyVehicleDataEntry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DailyTrailerInspectionClass TheDailyTrailerInspectionClass = new DailyTrailerInspectionClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        InspectionsClass TheInspectionsClass = new InspectionsClass();
        TrailersClass TheTrailersClass = new TrailersClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //Setting up the data
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindCurrentAssignedVehicleMainByVehicleIDDataSet TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = new FindCurrentAssignedVehicleMainByVehicleIDDataSet();
        FindActiveTrailerByTrailerNumberDataSet TheFindActiveTrailerByTrailerNumberDataSet = new FindActiveTrailerByTrailerNumberDataSet();

        //setting up global variables
        int gintVehicleID;
        int gintWarehouseEmployeeID;
        int gintEmployeeID;
        int gintTrailerID;
        DateTime gdatTransactionDate;
        bool gblnAssignVehicle;
        bool gblnDailyVehicleInspection;
        bool gblnDailyTrailerInspection;
        int gintTransactionID;
        string gstrInspectionStatus;

        public MainWindow()
        {
            InitializeComponent();
        }
        

        private void expClose_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void txtVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            string strVehicleNumber;
            int intLength;
            int intRecordsReturned;
            string strEmployeeGroup;
            string strFullName;

            try
            {
                strValueForValidation = txtEnterID.Text;
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Employee ID is not an Integer");
                    return;
                }
                else
                {
                    //getting warehouse id
                    gintWarehouseEmployeeID = Convert.ToInt32(strValueForValidation);
                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(gintWarehouseEmployeeID);

                    intRecordsReturned = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee ID Was Not Found");
                        return;
                    }
                    else
                    {
                        strEmployeeGroup = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmployeeGroup;

                        if(strEmployeeGroup != "WAREHOUSE")
                        {
                            TheMessagesClass.ErrorMessage("Employee Is Not a Warehouse Employee");
                            return;
                        }
                    }
                }

                strVehicleNumber = txtVehicleNumber.Text;
                intLength = strVehicleNumber.Length;
                if(intLength == 4)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                        TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleMainByVehicleID(gintVehicleID);

                        gintEmployeeID = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].EmployeeID;

                        strFullName = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].FirstName + " ";
                        strFullName += TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].LastName;

                        txtCurrentEmployee.Text = strFullName;
                        txtEnterLastName.IsEnabled = true;
                    }
                }
                else if(intLength == 6)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Not Found");
                        return;
                    }

                    gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleMainByVehicleID(gintVehicleID);

                    gintEmployeeID = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].EmployeeID;

                    strFullName = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].FirstName + " ";
                    strFullName += TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].LastName;

                    txtCurrentEmployee.Text = strFullName;
                    txtEnterLastName.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Main Window // Vehicle Number Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }

                        cboSelectEmployee.IsEnabled = true;
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Main Window // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
            cboBodyDamage.Items.Add("Select Body Damage");
            cboBodyDamage.Items.Add("Yes");
            cboBodyDamage.Items.Add("No");
            cboBodyDamage.SelectedIndex = 0;
            cboMultipleProblems.Items.Add("Select Multiple Problems");
            cboMultipleProblems.Items.Add("Yes");
            cboMultipleProblems.Items.Add("No");
            cboMultipleProblems.SelectedIndex = 0;
            cboNewProblem.Items.Add("Select New Problem");
            cboNewProblem.Items.Add("Yes");
            cboNewProblem.Items.Add("No");
            cboNewProblem.SelectedIndex = 0;
        }
        private void ResetControls()
        {
            txtEnterID.Text = "";
            txtVehicleNumber.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            chkAssignVehicle.IsChecked = false;
            chkDailyVehicleInspection.IsChecked = false;
            txtOdometer.Text = "";
            txtVehicleNumber.IsEnabled = false;
            txtEnterLastName.IsEnabled = false;
            cboSelectEmployee.IsEnabled = false;
            chkAssignVehicle.IsEnabled = false;
            chkDailyVehicleInspection.IsEnabled = false;
            txtOdometer.IsEnabled = false;
            cboBodyDamage.IsEnabled = false;
            rdoPassed.IsEnabled = false;
            rdoPassedServiceRequired.IsEnabled = false;
            expProcess.IsEnabled = false;
            chkDailyTrailerInspection.IsChecked = false;
            txtTrailerNumber.IsEnabled = false;
            cboTrailerDamageReported.IsEnabled = false;
            dgrTrailerProblems.IsEnabled = false;
            rdoTrailerPassed.IsEnabled = false;
            rdoTrailerPassedServiceRequired.IsEnabled = false;
            cboMultipleProblems.IsEnabled = false;
            cboNewProblem.IsEnabled = false;
            txtProblemNotes.IsEnabled = false;
            gblnAssignVehicle = false;
            gblnDailyTrailerInspection = false;
            gblnDailyVehicleInspection = false;
        }

        private void txtEnterID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtVehicleNumber.IsEnabled = true;
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                    chkAssignVehicle.IsEnabled = true;
                    chkDailyVehicleInspection.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Main Window // Select Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void chkAssignVehicle_Checked(object sender, RoutedEventArgs e)
        {
            gblnAssignVehicle = true;
            expProcess.IsEnabled = true;
            chkDailyTrailerInspection.IsChecked = false;
        }

        private void chkAssignVehicle_Unchecked(object sender, RoutedEventArgs e)
        {
            gblnAssignVehicle = false;
            expProcess.IsEnabled = false;
            chkDailyTrailerInspection.IsChecked = false;
        }

        private void chkDailyVehicleInspection_Checked(object sender, RoutedEventArgs e)
        {
            gblnDailyVehicleInspection = true;
            expProcess.IsEnabled = true;
            chkDailyTrailerInspection.IsChecked = false;
        }

        private void chkDailyVehicleInspection_Unchecked(object sender, RoutedEventArgs e)
        {
            gblnDailyVehicleInspection = false;
            expProcess.IsEnabled = false;
            chkDailyTrailerInspection.IsChecked = false;
        }

        private void chkDailyTrailerInspection_Checked(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            bool blnFatalError;
            int intRecordsReturned;
            string strEmployeeGroup;       
            
            strValueForValidation = txtEnterID.Text;
            blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage("The Employee ID is not an Integer");
                chkDailyTrailerInspection.IsChecked = false;
                return;
            }
            else
            {
                gintWarehouseEmployeeID = Convert.ToInt32(strValueForValidation);
                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(gintWarehouseEmployeeID);

                intRecordsReturned = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    chkDailyTrailerInspection.IsChecked = false;
                    return;
                }
                else
                {
                    strEmployeeGroup = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmployeeGroup;

                    if(strEmployeeGroup != "WAREHOUSE")
                    {
                        TheMessagesClass.ErrorMessage("This is not a Warehouse Employee, Please Use the Full ERP");
                        chkDailyTrailerInspection.IsChecked = false;
                        return;
                    }
                }
            }

            chkAssignVehicle.IsChecked = false;
            chkDailyVehicleInspection.IsChecked = false;
            txtTrailerNumber.IsEnabled = true;
            gblnDailyTrailerInspection = true;
            txtVehicleNumber.IsEnabled = false;
        }

        private void chkDailyTrailerInspection_Unchecked(object sender, RoutedEventArgs e)
        {
            txtTrailerNumber.IsEnabled = false;
            gblnDailyTrailerInspection = false;
        }

        private void rdoPassed_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED";
        }

        private void rdoPassedServiceRequired_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED SERVICE REQUIRED";
            VehicleInspectionProblem VehicleInspectionProblem = new VehicleInspectionProblem();
            VehicleInspectionProblem.ShowDialog();
        }

        private void rdoTrailerPassed_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED";
        }

        private void rdoTrailerPassedServiceRequired_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED SERVICE REQUIRED";
            cboMultipleProblems.IsEnabled = true;
            cboNewProblem.IsEnabled = true;
        }

        private void txtTrailerNumber_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
