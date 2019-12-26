/* Title:           Trailer Body Damage
 * Date:            12-26-19
 * Author:          Terry Hoilmes
 * 
 * Description:     This is used to report trailer damage */

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
using TrailerBodyDamageDLL;
using NewEventLogDLL;

namespace DailyVehicleDataEntry
{
    /// <summary>
    /// Interaction logic for TrailerBodyDamage.xaml
    /// </summary>
    public partial class TrailerBodyDamage : Window
    {
        //setting up the class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        TrailerBodyDamageClass TheTrailerBodyDamageClass = new TrailerBodyDamageClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindTrailerBodyDamageByTrailerIDDataSet TheFindTrailerBodyDamageByTrailerIDDataSet = new FindTrailerBodyDamageByTrailerIDDataSet();

        public TrailerBodyDamage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting up the controls
            cboDamageReported.Items.Add("Select Damage");
            cboDamageReported.Items.Add("Yes");
            cboDamageReported.Items.Add("No");
            cboDamageReported.SelectedIndex = 0;

            TheFindTrailerBodyDamageByTrailerIDDataSet = TheTrailerBodyDamageClass.FindTrailerBodyDamageByTrailerID(MainWindow.gintTrailerID);

            dgrReportedDamage.ItemsSource = TheFindTrailerBodyDamageByTrailerIDDataSet.FindTrailerBodyDamageByTrailerID;

            expProcess.IsEnabled = false;
        }

        private void cboDamageReported_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboDamageReported.SelectedIndex;

                if(intSelectedIndex == 1)
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Trailer Body Damage // Damaged Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
                if(intLength < 10)
                {
                    TheMessagesClass.ErrorMessage("The Damage Notes are not Long Enough");
                    return;
                }

                blnFatalError = TheTrailerBodyDamageClass.InsertTrailerBodyDamage(MainWindow.gintTrailerID, strNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Trailer Damages have been Added");

                this.Close();
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Daily Vehicle Data Entry // Trailer Body Damage // Process Expander " + Ex.Message))
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
