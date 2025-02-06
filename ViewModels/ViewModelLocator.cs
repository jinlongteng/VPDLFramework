using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;

namespace VPDLFramework.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<EditWorkViewModel>();
            SimpleIoc.Default.Register<LogViewModel>();
            SimpleIoc.Default.Register<FileManagerViewModel>();
            SimpleIoc.Default.Register<SystemSetupViewModel>();
            SimpleIoc.Default.Register<WorkRuntimeViewModel>();
            SimpleIoc.Default.Register<CommCardViewModel>();
        }

        /// <summary>
        /// ��������ͼģ��
        /// </summary>
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// ������ͼģ��
        /// </summary>
        public EditWorkViewModel Work
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditWorkViewModel>();
            }
        }


        /// <summary>
        /// ϵͳ��־��ͼģ��
        /// </summary>
        public LogViewModel Log
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LogViewModel>();
            }
        }

        /// <summary>
        /// �ļ�������ͼģ��
        /// </summary>
        public FileManagerViewModel FileManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FileManagerViewModel>();
            }
        }

        /// <summary>
        /// ϵͳ������ͼģ��
        /// </summary>
        public SystemSetupViewModel SystemSetup
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SystemSetupViewModel>();
            }
        }

        /// <summary>
        /// ͨѶ�忨��ͼģ��
        /// </summary>
        public CommCardViewModel CommCard
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CommCardViewModel>();
            }
        }

        /// <summary>
        /// ��������������ͼģ��
        /// </summary>
        public WorkRuntimeViewModel WorkRuntime
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WorkRuntimeViewModel>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
            SimpleIoc.Default.Unregister<MainViewModel>();
            SimpleIoc.Default.Unregister<EditWorkViewModel>();
            SimpleIoc.Default.Unregister<LogViewModel>();
            SimpleIoc.Default.Unregister<FileManagerViewModel>();
            SimpleIoc.Default.Unregister<SystemSetupViewModel>();
            SimpleIoc.Default.Unregister<WorkRuntimeViewModel>();
            SimpleIoc.Default.Unregister<CommCardViewModel>();
        }
    }
}