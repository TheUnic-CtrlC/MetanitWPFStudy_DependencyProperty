using System.Printing;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
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

namespace WpfAppMetanit
{
    //если хотим применять свойства зависимостей, то необходимо унаследовать свой класс от абстрактного класса DependencyProperty
    public class Phone : DependencyObject 
    {
        //определяем два свойства зависимостей
        //они объявляются с модификаторами public static readonly
        public static readonly DependencyProperty TitleProperty;
        public static readonly DependencyProperty PriceProperty;

        static Phone() //свойства регистрируются в статическом конструкторе с помощью метода Register
        {
            //добавление валидации. ValidateValueCallback - возвращает true, если валидация проходит.
            //CoerceValueCallback - делегат, который может подкорректировать уже существующее значение свойства
            //сначала срабатывает ValidateValueCallback, и если он возвращает true, то за ним срабатывает CoerceValueCallback
            //если оба делегата вернули true, то срабатывает деленат PropertyChangedCallback, который извещает об изменении значения свойства 
            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Phone)); //имя  тип  владелец  
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(); // FrameworkPropertyMetadata  устанавливает дополнительные настройки свойства. в данном случае хранит CoerceValueCallback
            metadata.CoerceValueCallback = new CoerceValueCallback(CorrectValue);
            PriceProperty = DependencyProperty.Register("Price", typeof(int), typeof(Phone),
                metadata, new ValidateValueCallback(ValidateValue)); // пятый (необязательный) параметр - ссылка на метод, который производит валидацию свойства
        }
        
        //делегат CoerceValueCallback возврашает будет вызывать CorrectValue 
        //данные метод принимает два параметра DependencyObject - валидируемый объект (в данном случает объект Phone)
        //и object - новое значение для свойства Price (либо для другого устанавливаемого свойства)
        //в самом методе CorrectValue мы получаем новое значение и модифицируем его, если оно некорректно 
        //при работе с делегатом CoerceValueCallback, надо учитывать, что он вызывается тогда, когда вызов делегата ValidateValueCallback возвращает true
        private static object CorrectValue(DependencyObject d, object baseValue)
        {
            int curValue = (int)baseValue;
            if (curValue > 1000) //если больше 1000, то возвращаем 1000
                return 1000;
            return curValue; //если меньше 1000, то возвращаем текущее значение
        }

        //Делегат ValidateValueCallback указывает на метод ValidateValue, который в качестве пар-ра принимает новое значение свойства
        private static bool ValidateValue(object value)
        {
            int curValue = (int)value;
            //необходимо установить >= 0, вместо > 0, тк изначально int выставляет дефолт 0 (и не сможет пройти валидацию), а уже потом Price получает значение 600 
            if (curValue >= 0) //если текущее значение от 0 и выше 
                return true; //если валидация пройдено, то возвращаем введенное значение
            return false; // если не пройдена, то возвращаем фолз и у нас выводится предыдущее корректно введенное значение
        }
        
        //и для них создаются свойства-обертки, в которых мы получаем доступ к значениям свойств с помоью GetValue и SetValue 
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set{ SetValue(TitleProperty, value); }
        }
        public int Price
        {
            get { return (int)GetValue(PriceProperty); }
            set {SetValue(PriceProperty, value); }
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Для проверки значения ресурса был создан обработчик нажатий
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Phone phone = (Phone)this.Resources["Sasung23S"]; //получаем ресурс по ключу
            MessageBox.Show(phone.Price.ToString());
        }
    }
}