using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nova.Base;
using Nova.Controls;

namespace Nova.Shell
{
    /// <summary>
    /// Interaction logic for TestPage.xaml
    /// </summary>
    public partial class TestPage
    {
        public TestPage()
        {
            InitializeComponent();
        }
    }

    public abstract class TestPageBase : ExtendedPage<TestPage, TestPageViewModel>
    {

    }

    public class TestPageViewModel : ViewModel<TestPage, TestPageViewModel> 
    {
        
    }
}
