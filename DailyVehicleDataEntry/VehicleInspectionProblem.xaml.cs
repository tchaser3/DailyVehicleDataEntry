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
using NewEmployeeDLL;
using NewEventLogDLL;
using VehicleProblemsDLL;
using InspectionsDLL;

namespace DailyVehicleDataEntry
{
    /// <summary>
    /// Interaction logic for VehicleInspectionProblem.xaml
    /// </summary>
    public partial class VehicleInspectionProblem : Window
    {
        //setting up the data
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        InspectionsClass TheInspectionClass = new InspectionsClass();

        FindVehicleProblemByDateMatchDataSet TheFindVehicleProblemByDateMatchDataSet = new FindVehicleProblemByDateMatchDataSet();
        FindOpenVehicleMainProblemsByVehicleIDDataSet TheFindOpenVehicleMainProblemsByVehicleIDDataSet = new FindOpenVehicleMainProblemsByVehicleIDDataSet();
        ExistingOpenProblemsDataSet TheExistingOpenProblemsDataSet = new ExistingOpenProblemsDataSet();
        FindVehicleMainProblemByProblemIDDataSet TheFindVehicleMainProblemByProblemIDDataSet = new FindVehicleMainProblemByProblemIDDataSet();
        FindEmployeeByDepartmentDataSet TheFindEmployeeByDepartmentDataSet = new FindEmployeeByDepartmentDataSet();
        FindVehicleInspectionProblemByProblemIDDataSet TheFindVehicleInspectionProblemByProblemIDDataSet = new FindVehicleInspectionProblemByProblemIDDataSet();
        FindSortedEmployeeManagersDataSet TheFindSortedEmployeeManagersDataSet = new FindSortedEmployeeManagersDataSet();

        int gintProblemID;
        bool gblnNewWorkOrder;
        bool gblnMultipleOrders;
        int gintManagerID;
        int gintFleetEmployeeID;
        string gstrVehicleProblem;

        public VehicleInspectionProblem()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strFullName;

            try
            {
                //setting up the controls
                cboNewProblem.Items.Add("Select New Problems");
                cboNewProblem.Items.Add("Yes");
                cboNewProblem.Items.Add("No");
                cboNewProblem.SelectedIndex = 0;

                cboMultipleProblems.Items.Add("Select Multiple Problems");
                cboMultipleProblems.Items.Add("Yes");
                cboMultipleProblems.Items.Add("No");
                cboMultipleProblems.SelectedIndex = 0;

                TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

                intNumberOfRecords = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;
                cboSelectManager.Items.Add("Select Manager");

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectManager.Items.Add(TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
                }

                cboSelectManager.SelectedIndex = 0;

                TheFindEmployeeByDepartmentDataSet = TheEmployeeClass.FindEmployeeByDepartment("WAREHOUSE");

                intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count - 1;

                cboSelectFleetEmployee.Items.Add("Select Fleet Employee");

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].LastName;

                    cboSelectFleetEmployee.Items.Add(strFullName);
                }

                cboSelectFleetEmployee.SelectedIndex = 0;

                TheFindOpenVehicleMainProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleMainProblemsByVehicleID(MainWindow.gintVehicleID);

                intNumberOfRecords = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        ExistingOpenProblemsDataSet.openproblemsRow NewProblemRow = TheExistingOpenProblemsDataSet.openproblems.NewopenproblemsRow();

                        NewProblemRow.Problem = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID[intCounter].Problem;
                        NewProblemRow.ProblemID = TheFindOpenVehicleMainProblemsByVehicleIDDataSet.FindOpenVehicleMainProblemsByVehicleID[intCounter].ProblemID;

                        TheExistingOpenProblemsDataSet.openproblems.Rows.Add(NewProblemRow);
                    }
                }

                dgrWorkOrders.ItemsSource = TheExistingOpenProblemsDataSet.openproblems;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Vehicle Inspection Problem // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboNewProblem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboNewProblem.SelectedIndex == 1)
                gblnNewWorkOrder = true;
            else if (cboNewProblem.SelectedIndex == 2)
                gblnNewWorkOrder = false;
        }

        private void cboMultipleProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMultipleProblems.SelectedIndex == 1)
                gblnMultipleOrders = true;
            else if (cboMultipleProblems.SelectedIndex == 2)
                gblnMultipleOrders = false;
        }
        private void SetReadOnlyControls(bool blnValueBoolean)
        {
            cboMultipleProblems.IsEnabled = blnValueBoolean;
            cboSelectFleetEmployee.IsEnabled = blnValueBoolean;
            cboSelectManager.IsEnabled = blnValueBoolean;
            txtFleetNotes.IsEnabled = blnValueBoolean;
            txtManagerNotes.IsEnabled = blnValueBoolean;
            txtInspectionNotes.IsEnabled = blnValueBoolean;
        }
        private bool CharacterCheck(string strValueForValidation)
        {
            bool blnItemFailed = false;
            char[] chaWordToTest;
            int intLength;
            int intCounter;
            int intPatternCounter = 0;
            char chaTestingCharacter = '*';

            chaWordToTest = strValueForValidation.ToCharArray();
            intLength = chaWordToTest.Length - 1;

            for (intCounter = 0; intCounter <= intLength; intCounter++)
            {
                if (chaTestingCharacter != chaWordToTest[intCounter])
                {
                    chaTestingCharacter = chaWordToTest[intCounter];
                    intPatternCounter = 0;
                }
                else if (chaTestingCharacter == chaWordToTest[intCounter])
                {
                    intPatternCounter += 1;
                    if (intPatternCounter > 3)
                    {
                        blnItemFailed = true;
                    }
                }
            }


            return blnItemFailed;
        }
        private void LoadComboBoxBoxes(int intManagerID, int intFleetEmployeeID)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            intNumberOfRecords = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if (TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].employeeID == gintManagerID)
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectManager.SelectedIndex = intSelectedIndex;

            intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if (TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].EmployeeID == gintFleetEmployeeID)
                {
                    intSelectedIndex = intCounter + 1;
                }
            }

            cboSelectFleetEmployee.SelectedIndex = intSelectedIndex;
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectManager.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintManagerID = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
            }
        }

        private void cboSelectFleetEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectFleetEmployee.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintFleetEmployeeID = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].EmployeeID;
            }
        }

        private void dgrWorkOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            DataGrid OpenOrderGrid;
            DataGridRow OpenOrderRow;
            DataGridCell ProblemID;
            string strProblemID;
            int intRecordsReturned;

            try
            {
                intSelectedIndex = dgrWorkOrders.SelectedIndex;
                //HideTextBoxes();

                if (intSelectedIndex > -1)
                {
                    SetReadOnlyControls(true);
                    OpenOrderGrid = dgrWorkOrders;
                    OpenOrderRow = (DataGridRow)OpenOrderGrid.ItemContainerGenerator.ContainerFromIndex(OpenOrderGrid.SelectedIndex);
                    ProblemID = (DataGridCell)OpenOrderGrid.Columns[0].GetCellContent(OpenOrderRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;

                    gintProblemID = Convert.ToInt32(strProblemID);

                    if (intSelectedIndex > 0)
                    {
                        TheFindVehicleMainProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleMainProblemByProblemID(gintProblemID);

                        gstrVehicleProblem = TheFindVehicleMainProblemByProblemIDDataSet.FindVehicleMainProblemByProblemID[0].Problem;

                        txtInspectionNotes.Text = gstrVehicleProblem;

                        txtInspectionNotes.IsReadOnly = true;
                    }

                    if (gintProblemID == -1)
                    {
                        cboSelectFleetEmployee.IsEnabled = true;
                        cboSelectManager.IsEnabled = true;
                        txtFleetNotes.Text = "";
                        txtManagerNotes.Text = "";
                        txtInspectionNotes.Text = "";
                        txtInspectionNotes.IsReadOnly = false;
                        gblnNewWorkOrder = true;
                        txtFleetNotes.IsReadOnly = false;
                        txtManagerNotes.IsReadOnly = false;
                    }
                    else if (gintProblemID > -1)
                    {
                        gblnNewWorkOrder = false;

                        TheFindVehicleInspectionProblemByProblemIDDataSet = TheInspectionClass.FindVehicleInspectionProblemByProblemID(gintProblemID);

                        intRecordsReturned = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            gintManagerID = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID[0].ManagerID;
                            gintFleetEmployeeID = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID[0].FleetEmployeeID;
                            txtFleetNotes.Text = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID[0].FleetEmployeeNotes;
                            txtManagerNotes.Text = TheFindVehicleInspectionProblemByProblemIDDataSet.FindVehicleInspectionProblemByProblemID[0].ManagerNotes;

                            LoadComboBoxBoxes(gintManagerID, gintFleetEmployeeID);
                        }
                    }


                    txtInspectionNotes.Visibility = Visibility.Visible;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Vehicle Inspection Problem // Open Problems Grid " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            //this will save the transaction
            string strInspectionNotes;
            bool blnFatalError = false;
            string strErrorMessage = "";
            DateTime datTransactionDate = DateTime.Now;
            //int intLength;
            string strManagerNotes;
            string strFleetNotes;
            bool blnThereIsaProblem = false;

            try
            {
                gstrVehicleProblem = txtInspectionNotes.Text;
                blnThereIsaProblem = CharacterCheck(gstrVehicleProblem);
                if (blnThereIsaProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "There Is a Recurring Character that is displaying, Invalid Entry\n";
                }
                strManagerNotes = txtManagerNotes.Text;
                if (strManagerNotes.Length < 20)
                {
                    blnFatalError = true;
                    strErrorMessage += "Manager Notes Entry is not Long Enough\n";
                }
                strFleetNotes = txtFleetNotes.Text;
                if (strFleetNotes.Length < 20)
                {
                    blnFatalError = true;
                    strErrorMessage += "Fleet Notes Entry is not Long Enough\n";
                }
                if (cboSelectFleetEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Fleet Employee Was Not Selected\n";
                }
                if (cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Manager Was Not Selected\n";
                }
                if (cboMultipleProblems.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Multiple Orders Was Not Selected\n";
                }
                if (gblnNewWorkOrder == true)
                {
                    if (gstrVehicleProblem == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "Vehicle Problem Was Not Entered\n";
                    }
                }

                strInspectionNotes = txtInspectionNotes.Text;
                if (strInspectionNotes == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Inspection Notes Were Not Entered\n";
                }
                else
                {
                    blnThereIsaProblem = CharacterCheck(strInspectionNotes);
                    if (blnThereIsaProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "There is a recurring character in the Inspection Notes\n";
                    }
                }

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                strManagerNotes = "MANAGER NOTES: " + strManagerNotes;
                strFleetNotes = "FLEET NOTES: " + strFleetNotes;

                if (gblnNewWorkOrder == true)
                {
                    //iserting into table
                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblem(MainWindow.gintVehicleID, datTransactionDate, gstrVehicleProblem);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindVehicleProblemByDateMatchDataSet = TheVehicleProblemClass.FindVehicleProblemByDateMatch(datTransactionDate);

                    gintProblemID = TheFindVehicleProblemByDateMatchDataSet.FindVehicleProblemByDateMatch[0].ProblemID;

                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(gintProblemID, gintFleetEmployeeID, strManagerNotes, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(gintProblemID, gintFleetEmployeeID, strFleetNotes, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheInspectionClass.InsertVehicleInspectionProblem(MainWindow.gintVehicleID, MainWindow.gintInspectionID, "DAILY", gstrVehicleProblem, MainWindow.gintOdometerReading, MainWindow.gblnServicable, strInspectionNotes, gintManagerID, gintFleetEmployeeID, strManagerNotes, strFleetNotes, gintProblemID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Vehicle Has Been Updated");

                if (gblnMultipleOrders == true)
                {
                    txtInspectionNotes.Text = "";
                    txtManagerNotes.Text = "";
                    txtFleetNotes.Text = "";
                    cboMultipleProblems.SelectedIndex = 0;
                    SetReadOnlyControls(false);

                }
                else if (gblnMultipleOrders == false)
                {

                    Close();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Vehicle Inspection Problem // Save Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
