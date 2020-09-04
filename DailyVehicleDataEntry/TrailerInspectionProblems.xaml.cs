/* Title:           Trailer Inspection Problems
 * Date:            9-4-2020
 * Author:          Terry Holmes
 * 
 * Description:     this is used for reporting trailer inspection problems */

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
using TrailerProblemDLL;
using TrailerProblemUpdateDLL;

namespace DailyVehicleDataEntry
{
    /// <summary>
    /// Interaction logic for TrailerInspectionProblems.xaml
    /// </summary>
    public partial class TrailerInspectionProblems : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        TrailerProblemClass TheTrailerProblemClass = new TrailerProblemClass();
        TrailerProblemUpdateClass TheTrailerProblemUpdateClass = new TrailerProblemUpdateClass();

        FindOpenTrailerProblemsByTrailerIDDataSet TheFindOpenTrailerProblemsByTrailerIDDataSet = new FindOpenTrailerProblemsByTrailerIDDataSet();
        ExistingTrailerProblemsDataSet TheExistingTrailerProblemsDataSet = new ExistingTrailerProblemsDataSet();
        FindTrailerProblemByDateMatchDataSet TheFindTrailerProblemByDateMatchDataSet = new FindTrailerProblemByDateMatchDataSet();

        public TrailerInspectionProblems()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboNewProblem.Items.Add("Select");
                cboNewProblem.Items.Add("Yes");
                cboNewProblem.Items.Add("No");
                cboNewProblem.SelectedIndex = 0;

                dgrCurrentProblems.IsEnabled = false;
                MainWindow.gintProblemID = -1;

                TheFindOpenTrailerProblemsByTrailerIDDataSet = TheTrailerProblemClass.FindOpenTrailerProblemsByTrailerID(MainWindow.gintTrailerID);

                intNumberOfRecords = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID.Rows.Count - 1;                

                if (intNumberOfRecords < 0)
                {
                    cboNewProblem.SelectedIndex = 1;
                    cboNewProblem.IsEnabled = false;
                    dgrCurrentProblems.IsEnabled = false;
                }
                else
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        ExistingTrailerProblemsDataSet.existingtrailerproblemsRow NewProblemRow = TheExistingTrailerProblemsDataSet.existingtrailerproblems.NewexistingtrailerproblemsRow();

                        NewProblemRow.ProblemID = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].ProblemID;
                        NewProblemRow.ProblemReported = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].ProblemReported;
                        NewProblemRow.TransactionDate = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID[intCounter].TransactionDate;

                        TheExistingTrailerProblemsDataSet.existingtrailerproblems.Rows.Add(NewProblemRow);
                    }
                }

                dgrCurrentProblems.ItemsSource = TheExistingTrailerProblemsDataSet.existingtrailerproblems;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Trailer Inspection Problems // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboNewProblem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboNewProblem.SelectedIndex == 1)
            {
                dgrCurrentProblems.IsEnabled = false;
            }
            else if (cboNewProblem.SelectedIndex == 2)
            {
                dgrCurrentProblems.IsEnabled = true;
            }
        }

        private void dgrCurrentProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TrailerProblem;
            string strTrailerProblem;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                intSelectedIndex = dgrCurrentProblems.SelectedIndex;

                if (intSelectedIndex > -1)
                {
                    dataGrid = dgrCurrentProblems;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    TrailerProblem = (DataGridCell)dataGrid.Columns[2].GetCellContent(selectedRow).Parent;
                    strTrailerProblem = ((TextBlock)TrailerProblem.Content).Text;
                    ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    txtEnterProblem.Text = strTrailerProblem;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Trailer Inspection Problem // Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            string strTrailerProblem;
            string strErrorMessage = "";
            int intTrailerProblemLength;

            try
            {
                if (cboNewProblem.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "New Problem Was Not Selected\n";
                }
                strTrailerProblem = txtEnterProblem.Text;
                intTrailerProblemLength = strTrailerProblem.Length;
                if (intTrailerProblemLength < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Entered Is Not Long Enough\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if (cboNewProblem.SelectedIndex == 1)
                {
                    blnFatalError = TheTrailerProblemClass.InsertTrailerProblem(MainWindow.gintTrailerID, datTransactionDate, MainWindow.gintWarehouseEmployeeID, strTrailerProblem, 1001);

                    if (blnFatalError == true)
                    {
                        throw new Exception();
                    }

                    TheFindTrailerProblemByDateMatchDataSet = TheTrailerProblemClass.FindTrailerProblemByDateMatch(datTransactionDate);

                    MainWindow.gintProblemID = TheFindTrailerProblemByDateMatchDataSet.FindTrailerProblemByDateMatch[0].ProblemID;
                }

                if (MainWindow.gintProblemID == -1)
                {
                    TheMessagesClass.ErrorMessage("A Problem was not Selected");
                    return;
                }

                blnFatalError = TheTrailerProblemUpdateClass.InsertTrailerProblemUpdate(MainWindow.gintProblemID, MainWindow.gintWarehouseEmployeeID, strTrailerProblem);

                if (blnFatalError == true)
                {
                    throw new Exception();
                }

                TheMessagesClass.InformationMessage("Problem Was Entered");

                MainWindow.gstrInspectionProblem += strTrailerProblem + "\n";

                MainWindow.gintProblemID = 0;
                txtEnterProblem.Text = "";

                TheFindOpenTrailerProblemsByTrailerIDDataSet = TheTrailerProblemClass.FindOpenTrailerProblemsByTrailerID(MainWindow.gintTrailerID);

                dgrCurrentProblems.ItemsSource = TheFindOpenTrailerProblemsByTrailerIDDataSet.FindOpenTrailerProblemByTrailerID;

                cboNewProblem.IsEnabled = true;
                cboNewProblem.SelectedIndex = 0;
                Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay Communications // Inspection Trailer Problem // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
