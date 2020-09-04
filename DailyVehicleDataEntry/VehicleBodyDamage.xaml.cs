/* Title:           Vehicle Body Damage
 * Date:            8-18-20
 * Author:          Terry Holmes
 * 
 * Description:     This used for vehicle body damage */

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
using VehicleBodyDamageDLL;
using NewEventLogDLL;

namespace DailyVehicleDataEntry
{
    /// <summary>
    /// Interaction logic for VehicleBodyDamage.xaml
    /// </summary>
    public partial class VehicleBodyDamage : Window
    {
        //sertting up the clases
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleBodyDamageClass TheVehicleBodyDamageClass = new VehicleBodyDamageClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindVehicleMainBodyDamageByVehicleIDDataSet TheFindVehicleMainBodyDamageByVehicleIDDataSet = new FindVehicleMainBodyDamageByVehicleIDDataSet();

        public VehicleBodyDamage()
        {
            InitializeComponent();
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            //this method will insert the trailer damage
            bool blnFatalError = false;
            string strNotes;
            int intLength;

            try
            {
                strNotes = txtDamageNotes.Text;
                intLength = strNotes.Length;
                if (intLength < 10)
                {
                    TheMessagesClass.ErrorMessage("The Damage Notes are not Long Enough");
                    return;
                }

                blnFatalError = TheVehicleBodyDamageClass.InsertNewVehicleBodyDamage(MainWindow.gintVehicleID, strNotes, DateTime.Now, false);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Vehicled Damages have been Added");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Vehicle Body Damage // Process Expander " + Ex.Message);
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting up the controls
            cboDamageReported.Items.Add("Select Damage");
            cboDamageReported.Items.Add("Yes");
            cboDamageReported.Items.Add("No");
            cboDamageReported.SelectedIndex = 0;

            TheFindVehicleMainBodyDamageByVehicleIDDataSet = TheVehicleBodyDamageClass.FindVehicleMainBodyDamageByVehicleID(MainWindow.gintVehicleID);

            dgrReportedDamage.ItemsSource = TheFindVehicleMainBodyDamageByVehicleIDDataSet.FindVehicleMainBodyDamageByVehicleID;

            expProcess.IsEnabled = false;
        }

        private void cboDamageReported_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboDamageReported.SelectedIndex;

                if (intSelectedIndex == 1)
                {
                    this.Close();
                }
                else
                {
                    expProcess.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Vehicle Body Damage // Damaged Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
