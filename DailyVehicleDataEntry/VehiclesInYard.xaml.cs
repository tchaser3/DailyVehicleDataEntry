/* Title:           Vehicles In Yard
 * Date:            9-16-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used to enter the vehicles in the yard */

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
using VehicleMainDLL;
using VehicleInYardDLL;
using NewEventLogDLL;
using DateSearchDLL;

namespace DailyVehicleDataEntry
{
    /// <summary>
    /// Interaction logic for VehiclesInYard.xaml
    /// </summary>
    public partial class VehiclesInYard : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        VehicleInYardClass TheVehicleInYardClass = new VehicleInYardClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up data
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindVehiclesInYardByVehicleIDAndDateRangeDataSet TheFindVehiclesInYardByVehicleIDAndDateRangeDataSet = new FindVehiclesInYardByVehicleIDAndDateRangeDataSet();

        public VehiclesInYard()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtVehicleNumber.Text = "";
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strVehicleNumber;
            int intRecordsReturned;
            int intVehicleID;
            DateTime datTransactionDate;
            bool blnFatalError = false;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            try
            {
                strVehicleNumber = txtVehicleNumber.Text;
                datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);

                if (strVehicleNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Vehicle Number Was Not Entered");
                    return;
                }

                TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(strVehicleNumber);

                intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("The Vehicle Number Entered Does Not Exist");
                    return;
                }
                else if (intRecordsReturned == 1)
                {
                    intVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                    TheFindVehiclesInYardByVehicleIDAndDateRangeDataSet = TheVehicleInYardClass.FindVehiclesInYardByVehicleIDAndDateRange(intVehicleID, datStartDate, datEndDate);

                    intRecordsReturned = TheFindVehiclesInYardByVehicleIDAndDateRangeDataSet.FindVehiclesInYardByVehicleIDAndDateRange.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Has Already Been Placed In The Yard");
                        return;
                    }

                    datTransactionDate = DateTime.Now;

                    blnFatalError = TheVehicleInYardClass.InsertVehicleInYard(datTransactionDate, intVehicleID);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Was a Problem, Contact IT");
                        return;
                    }
                }

                txtVehicleNumber.Text = "";

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily  // Vehicle In Yard // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expClose_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
