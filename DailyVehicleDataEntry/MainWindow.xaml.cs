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
using TrailerProblemDLL;
using TrailerProblemUpdateDLL;
using InspectGPSDLL;
using VehicleProblemsDLL;
using TrailerHistoryDLL;

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
        TrailerProblemClass TheTrailerProblemClass = new TrailerProblemClass();
        TrailerProblemUpdateClass TheTrailerProblemUpdateClas = new TrailerProblemUpdateClass();
        InspectGPSClass TheInspectGPSClass = new InspectGPSClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        TrailerHistoryClass TheTrailerHistoryClass = new TrailerHistoryClass();

        //Setting up the data
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindCurrentAssignedVehicleMainByVehicleIDDataSet TheFindCurrentAssignedVehicleMainByVehicleIDDataSet = new FindCurrentAssignedVehicleMainByVehicleIDDataSet();
        FindActiveTrailerByTrailerNumberDataSet TheFindActiveTrailerByTrailerNumberDataSet = new FindActiveTrailerByTrailerNumberDataSet();
        FindOpenTrailerProblemsByTrailerIDDataSet TheFindOpenTrailerProblemsByTrailerIDDataSet = new FindOpenTrailerProblemsByTrailerIDDataSet();
        FindCurrentVehicleAssignmentByEmployeeIDDataSet TheFindCurrentVehicleAssignmentByEmployeeIDDataSet = new FindCurrentVehicleAssignmentByEmployeeIDDataSet();
        FindDailyVehicleInspectionForGPSDataSet TheFindDailyVehicleInspectionForGPSDataSet = new FindDailyVehicleInspectionForGPSDataSet();
        FindAllVehicleMainProblemsByVehicleIDDataSet TheFindVehicleMainProblemsByVehicleIDDataSet = new FindAllVehicleMainProblemsByVehicleIDDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindTrailerByEmployeeIDDataSet TheFindTrailerByEmployeeIDDataSet = new FindTrailerByEmployeeIDDataSet();
        

        //setting up global variables
        public static int gintVehicleID;
        public static int gintWarehouseEmployeeID;
        public static int gintEmployeeID;
        public static int gintTrailerID;
        public static DateTime gdatTransactionDate;
        public static bool gblnAssignVehicle;
        public static bool gblnDailyVehicleInspection;
        public static bool gblnDailyTrailerInspection;
        public static int gintTransactionID;
        public static string gstrInspectionStatus;
        public string gstrVehicleNumber;
        public static bool gblnRecordSaved;
        bool gblnGPSInstalled;
        bool gblnTodaysDate;
        public static int gintInspectionID;
        public static int gintOdometerReading;
        public static bool gblnServicable;
        public static int gintProblemID;
        public static string gstrInspectionProblem;
        public static string gstrHomeOffice;

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
                    if (gblnRecordSaved == false)
                    {
                        TheMessagesClass.ErrorMessage("The Employee ID is not an Integer");
                        gblnRecordSaved = false;
                        return;
                    }
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
                            if(strEmployeeGroup != "ADMIN")
                            {
                                if(strEmployeeGroup != "MANAGERS")
                                {
                                    TheMessagesClass.ErrorMessage("Employee Is Not a Warehouse Employee");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            expAssignTask.IsEnabled = true;
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
                else if(intLength > 6)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Is Not Formated Correctly");
                    return;
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
                    chkAssignVehicle.IsEnabled = true;
                    chkDailyVehicleInspection.IsEnabled = true;
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
            cboTrailerDamageReported.Items.Add("Select Trailer Damage");
            cboTrailerDamageReported.Items.Add("Yes");
            cboTrailerDamageReported.Items.Add("No");
            cboTrailerDamageReported.SelectedIndex = 0;
        }
        private void ResetControls()
        {
            chkTodaysDate.IsChecked = true;
            txtVehicleNumber.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            chkAssignVehicle.IsChecked = false;
            chkDailyVehicleInspection.IsChecked = false;
            txtOdometer.Text = "";
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
            rdoTrailerPassed.IsEnabled = false;
            rdoTrailerPassedServiceRequired.IsEnabled = false;
            gblnAssignVehicle = false;
            gblnDailyTrailerInspection = false;
            gblnDailyVehicleInspection = false;
            expAssignTask.IsEnabled = false;
            txtTrailerCurrentEmployee.IsEnabled = false;
            txtTrailerEnterLastName.IsEnabled = false;
            txtCurrentEmployee.Text = "";
            txtTrailerNumber.Text = "";
            txtTrailerEnterLastName.Text = "";
            rdoTrailerPassed.IsChecked = false;
            rdoTrailerPassedServiceRequired.IsChecked = false;
            cboTrailerSelectEmployee.Items.Clear();
            txtTrailerCurrentEmployee.Text = "";
            cboTrailerDamageReported.SelectedIndex = 0;
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
            txtOdometer.IsEnabled = true;
            cboBodyDamage.IsEnabled = true;
            rdoPassed.IsEnabled = true;
            rdoPassedServiceRequired.IsEnabled = true;
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
                if (gblnRecordSaved == false)
                {
                    TheMessagesClass.ErrorMessage("The Employee ID is not an Integer");
                    gblnRecordSaved = false;
                    chkDailyTrailerInspection.IsChecked = false;
                    return;
                }
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
                        if(strEmployeeGroup != "ADMIN")
                        {
                            if(strEmployeeGroup != "MANAGERS")
                            {
                                TheMessagesClass.ErrorMessage("This is not a Warehouse Employee, Please Use the Full ERP");
                                chkDailyTrailerInspection.IsChecked = false;
                                return;
                            }
                        }
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
            txtVehicleNumber.IsEnabled = true;
            
        }

        private void rdoPassed_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED";
            gblnServicable = true;
        }

        private void rdoPassedServiceRequired_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED SERVICE REQUIRED";
            gblnServicable = true;
        }

        private void rdoTrailerPassed_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED";
            expProcess.IsEnabled = true;
        }

        private void rdoTrailerPassedServiceRequired_Checked(object sender, RoutedEventArgs e)
        {
            gstrInspectionStatus = "PASSED SERVICE REQUIRED";
            expProcess.IsEnabled = true;
        }

        private void txtTrailerNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strTrailerNumber;
            int intLength;
            int intRecordsReturned;
            string strFullName;

            try
            {
                strTrailerNumber = txtTrailerNumber.Text;
                intLength = strTrailerNumber.Length;

                if(intLength == 4)
                {
                    TheFindActiveTrailerByTrailerNumberDataSet = TheTrailersClass.FindActiveTrailerByTrailerNumber(strTrailerNumber);

                    intRecordsReturned = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        gintTrailerID = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerID;

                        strFullName = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].FirstName + " ";
                        strFullName += TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].LastName;
                        txtTrailerCurrentEmployee.Text = strFullName;

                        txtTrailerEnterLastName.IsEnabled = true;
                    }
                    else if (intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("Trailer Number Not Found");
                        return;
                    }
                }
                else if(intLength == 6)
                {
                    TheFindActiveTrailerByTrailerNumberDataSet = TheTrailersClass.FindActiveTrailerByTrailerNumber(strTrailerNumber);

                    intRecordsReturned = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        gintTrailerID = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].TrailerID;
                        strFullName = TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].FirstName + " ";
                        strFullName += TheFindActiveTrailerByTrailerNumberDataSet.FindActiveTrailerByTrailerNumber[0].LastName;
                        txtTrailerCurrentEmployee.Text = strFullName;

                        txtTrailerEnterLastName.IsEnabled = true;
                    }
                    else if(intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("Trailer Number Not Found");
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Main Window // Trailer Number Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboTrailerDamageReported_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboTrailerDamageReported.SelectedIndex;

                if(intSelectedIndex == 1)
                {
                    TrailerBodyDamage TrailerBodyDamage = new TrailerBodyDamage();
                    TrailerBodyDamage.ShowDialog();
                }

                rdoTrailerPassed.IsEnabled = true;
                rdoTrailerPassedServiceRequired.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Main Window // Trailer Damage Combobox " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchEmail();
        }

        private void expAssignTask_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.AddTask();
        }

        private void txtTrailerEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtTrailerEnterLastName.Text;
                intLength = strLastName.Length;

                if (intLength > 2)
                {
                    cboTrailerSelectEmployee.Items.Clear();
                    cboTrailerSelectEmployee.Items.Add("Select Employee");

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboTrailerSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }

                        cboTrailerSelectEmployee.IsEnabled = true;
                    }

                    cboTrailerSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Main Window // Enter Trailer Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboTrailerSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboTrailerSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;                   

                    cboTrailerDamageReported.IsEnabled = true;
                    
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Main Window // Select Trailer Employee Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            expProcess.IsExpanded = false;

            try
            {
                if (gblnDailyTrailerInspection == true)
                {
                    blnFatalError = ProcessTrailerInspection();
                }
                if (gblnDailyVehicleInspection == true)
                {
                    blnFatalError = ProcessDailyVehicleInspection();
                }
                if ((gblnAssignVehicle == true) && (blnFatalError == false))
                {
                    blnFatalError = VehicleAssignment();
                }

                if(blnFatalError == false)
                {
                    gblnRecordSaved = true;
                    TheMessagesClass.InformationMessage("The Records Has Been Updated");
                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }           
        }
        private bool ProcessTrailerInspection()
        {
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            string strErrorMessage = "";
            int intRecordsReturned;
            int intWarehouseID;
            int intTrailerID;
            string strHomeOffice;

            try
            {
                if (MainWindow.gintTrailerID == -1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Trailer Was Not Entered\n";
                }
                if (cboTrailerSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Employee Was Not Selected\n";
                }
                if (cboTrailerDamageReported.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Damage Reported Was Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return blnFatalError;
                }

                TheFindTrailerByEmployeeIDDataSet = TheTrailersClass.FindTrailerByEmployeeID(gintEmployeeID);

                intRecordsReturned = TheFindTrailerByEmployeeIDDataSet.FindTrailerByEmployeeID.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    if(gintTrailerID != TheFindTrailerByEmployeeIDDataSet.FindTrailerByEmployeeID[0].TrailerID)
                    {
                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(gintEmployeeID);

                        if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName != "WAREHOUSE")
                        {
                            const string message = "The Employee Is Already Assigned to a Trailer\nDo You Want To Sign Them Out";
                            const string caption = "Are You Sure";
                            MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                intTrailerID = TheFindTrailerByEmployeeIDDataSet.FindTrailerByEmployeeID[0].TrailerID;



                                strHomeOffice = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].HomeOffice;

                                intWarehouseID = FindWarehouseID(strHomeOffice);

                                blnFatalError = TheTrailersClass.UpdateTrailerEmployeeAndAvailability(intTrailerID, intWarehouseID, true);

                                if (blnFatalError == true)
                                    throw new Exception();
                            }
                        }
                    }
                    
                }

                if (gstrInspectionStatus == "PASSED")
                {
                    MainWindow.gstrInspectionProblem = "NO PROBLEMS REPORTED";
                }
                else
                {
                    TrailerInspectionProblems TrailerInspectionProblems = new TrailerInspectionProblems();
                    TrailerInspectionProblems.ShowDialog();
                }

                blnFatalError = TheDailyTrailerInspectionClass.InsertDailyTrailerInspection(MainWindow.gintTrailerID, MainWindow.gintEmployeeID, datTransactionDate, gstrInspectionStatus, MainWindow.gstrInspectionProblem);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheTrailerHistoryClass.InsertTrailerHistory(MainWindow.gintTrailerID, MainWindow.gintEmployeeID, gintWarehouseEmployeeID, "TRAILER SIGNED OUT");

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheTrailersClass.UpdateTrailerEmployeeAndAvailability(MainWindow.gintTrailerID, gintEmployeeID, true);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Trailer Inspection Has Been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Daily Trailer Inspection // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private bool VehicleAssignment()
        {
            //data validation
            string strValueForValidation;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            int intTransactionID;
            string strMessage;
            string strCaption = "Thank You";
            string strHomeOffice = "";
            string strSecondVehicleNumber;
            int intVehicleID;
            int intWarehouseID;

            try
            {
                gstrVehicleNumber = txtVehicleNumber.Text;

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(gstrVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehice Number Not Valid\n";
                }
                else
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    strHomeOffice = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].AssignedOffice;
                }

                strValueForValidation = cboSelectEmployee.SelectedItem.ToString();
                if (strValueForValidation == "Select Employee")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return blnFatalError;
                }

                TheFindCurrentVehicleAssignmentByEmployeeIDDataSet = TheVehicleAssignmentClass.FindCurrentVehicleAssignmentByEmployeeID(MainWindow.gintEmployeeID);

                intRecordsReturned = TheFindCurrentVehicleAssignmentByEmployeeIDDataSet.FindCurrentVehicleMainAssignmentByEmployeeID.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    if (TheFindCurrentVehicleAssignmentByEmployeeIDDataSet.FindCurrentVehicleMainAssignmentByEmployeeID[0].LastName != "WAREHOUSE")
                    {
                        intTransactionID = TheFindCurrentVehicleAssignmentByEmployeeIDDataSet.FindCurrentVehicleMainAssignmentByEmployeeID[0].TransactionID;

                        strMessage = "Employee Is Assigned To Vehicle Number " + TheFindCurrentVehicleAssignmentByEmployeeIDDataSet.FindCurrentVehicleMainAssignmentByEmployeeID[0].VehicleNumber + "\nDo You Want to Sign Out the Employee";

                        MessageBoxResult result = MessageBox.Show(strMessage, strCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            intWarehouseID = FindWarehouseID(strHomeOffice);

                            strSecondVehicleNumber = TheFindCurrentVehicleAssignmentByEmployeeIDDataSet.FindCurrentVehicleMainAssignmentByEmployeeID[0].VehicleNumber;

                            TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strSecondVehicleNumber);

                            intVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                            blnFatalError = TheVehicleAssignmentClass.UpdateCurrentVehicleAssignment(intTransactionID, false);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheVehicleMainClass.UpdateVehicleMainEmployeeID(intVehicleID, intWarehouseID);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(intVehicleID, intWarehouseID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                blnFatalError = TheVehicleMainClass.UpdateVehicleMainEmployeeID(MainWindow.gintVehicleID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(MainWindow.gintVehicleID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();


                intRecordsReturned = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID.Rows.Count;

                if (intRecordsReturned == 1)
                {
                    intTransactionID = TheFindCurrentAssignedVehicleMainByVehicleIDDataSet.FindCurrentAssignedVehicleMainByVehicleID[0].TransactionID;

                    blnFatalError = TheVehicleAssignmentClass.UpdateCurrentVehicleAssignment(intTransactionID, false);

                    if (blnFatalError == true)
                    {
                        throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Record Has Been Saved");
                txtVehicleNumber.Focus();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Vehicle Assignment // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private int FindWarehouseID(string strWarehouseName)
        {
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID = 0;

            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(strWarehouseName == TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                {
                    intWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;  
                }
            }

            return intWarehouseID;
        }

        private void rdoInstalledYes_Checked(object sender, RoutedEventArgs e)
        {
            gblnGPSInstalled = true;
        }

        private void rdoInstalledNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnGPSInstalled = false;
        }
        private bool ProcessDailyVehicleInspection()
        {
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            DateTime datTransactionDate = DateTime.Now;
            int intRecordsReturned;
            string strVehicleNumber;

            try
            {
                strValueForValidation = txtEnterDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date is not a Date\n";
                }
                else
                {
                    datTransactionDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEnterID.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                strVehicleNumber = txtVehicleNumber.Text;
                if(strVehicleNumber.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "Vehicle Number is not Long Enough\n";
                }
                else
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Vehicle Was Not Found\n";
                    }
                    else
                    {
                        gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;
                    }
                }
                strValueForValidation = txtOdometer.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Odometer is not a Integer\n";
                }
                else
                {
                    gintOdometerReading = Convert.ToInt32(strValueForValidation);
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return blnFatalError;
                }

                TheFindDailyVehicleInspectionForGPSDataSet = TheInspectionsClass.FindDailyVehicleInspectionForGPS(gintVehicleID, datTransactionDate, gintEmployeeID, gstrInspectionStatus, gintOdometerReading);

                intRecordsReturned = TheFindDailyVehicleInspectionForGPSDataSet.FindDailyVehicleInspectionForGPS.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    blnFatalError = true;
                    TheMessagesClass.ErrorMessage("This Inspection Has Been Entered\n");
                    return blnFatalError;
                }

                if (gstrInspectionStatus == "PASSED")
                {
                    TheFindVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindAllVehicleMainProblemsByVehicleID(gintVehicleID);

                    intRecordsReturned = TheFindVehicleMainProblemsByVehicleIDDataSet.FindAllVehicleMainProblemsByVehicleID.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        blnFatalError = true;
                        TheMessagesClass.ErrorMessage("There are Open Vehicle Problems, The Inspection Will Not Be Processed");
                        return blnFatalError;
                    }
                }

                blnFatalError = TheInspectionsClass.InsertDailyVehicleInspection(gintVehicleID, datTransactionDate, gintEmployeeID, gstrInspectionStatus, gintOdometerReading);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindDailyVehicleInspectionForGPSDataSet = TheInspectionsClass.FindDailyVehicleInspectionForGPS(gintVehicleID, datTransactionDate, gintEmployeeID, gstrInspectionStatus, gintOdometerReading);

                gintInspectionID = TheFindDailyVehicleInspectionForGPSDataSet.FindDailyVehicleInspectionForGPS[0].TransactionID;

                blnFatalError = TheInspectGPSClass.InsertInspectGPS(gintInspectionID, "DAILY", gblnGPSInstalled);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheVehicleMainClass.UpdateVehicleMainEmployeeID(gintVehicleID, gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();                            

                if (gstrInspectionStatus == "PASSED SERVICE REQUIRED")
                {
                    VehicleInspectionProblem VehicleInspectionProblem = new VehicleInspectionProblem();
                    VehicleInspectionProblem.ShowDialog();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Main Window // Process Daily Vehicle Inspection " + Ex.Message);

                blnFatalError = true;
            }

            return blnFatalError;
        }

        private void chkTodaysDate_Checked(object sender, RoutedEventArgs e)
        {
            gblnTodaysDate = true;
            txtEnterDate.Text = Convert.ToString(DateTime.Now);
            txtEnterDate.IsEnabled = false;

        }

        private void chkTodaysDate_Unchecked(object sender, RoutedEventArgs e)
        {
            gblnTodaysDate = true;
            txtEnterDate.IsEnabled = true;
            txtEnterDate.Text = "";
        }

        private void cboBodyDamage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboBodyDamage.SelectedIndex == 1)
            {
                VehicleBodyDamage VehicleBodyDamage = new VehicleBodyDamage();
                VehicleBodyDamage.ShowDialog();
            }
        }
    }
}
