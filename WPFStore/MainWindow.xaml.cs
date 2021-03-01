using System;
using System.Collections.Generic;
using System.IO;
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

namespace WPFStore
{
    public partial class MainWindow : Window
    {
        public class Product //Data for products
        {
            public string ID;
            public string Name;
            public string Description;
            public decimal Price;

            public Product(string id, string name, string description, decimal price)
            {
                ID = id;
                Name = name;
                Description = description;
                Price = price;
            }
        }
        public class Cart //Data for cartItems and cart
        {
            //public string ID;
            public string Name;
            public string Description;
            public decimal Price;
            public int Quantity;

            public Cart(string name, string description, decimal price, int quantity)
            {
                //ID = id;
                Name = name;
                Description = description;
                Price = price;
                Quantity = quantity;
               
            }
        }
        //instance variables
        public List<Product> products = new List<Product>(); //used in STORE
        public List<Cart> carts = new List<Cart>(); //used in CART (to save carts)
        public List<Cart> currentCart = new List<Cart>(); //used in CART (contains items for current cart)
        public WrapPanel gallery; //used in STORE (shows productgallery)
        public Button pixButton, addToCart, deleteCartItem, addCartItemQuantity, removeCartItemQuantity; //pixbutton and addToCart in STORE, others in CART
        public Label productInfo, quantity, price, totalCartPriceLabel ; //used in STORE
        public Grid cartItemsGrid; //used in CART. Grid that build the pseudo-datagrid of the cart
        public StackPanel cartPanel; //used in CART. Container for above grids.
        public List<string> productNamesForComparison = new List<string>();
        public decimal totalCartPrice = 0;
        public TextBox discountCodeBox;
        //public int counter = 0;
        //public int cartItemQuantity = 1;


        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "School Project";
            Width = 1050;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.ShowGridLines = true;

            //loads product-info into the list "products"
            LoadProductList();

            #region Store 
            //region that encompasses the store-part of the GUI

            Label storeHeader = new Label
            {
                Content = "Welcome to the Ps5 Store",
                FontSize = 25,
                FontWeight = FontWeights.Bold,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(storeHeader);
            Grid.SetColumn(storeHeader, 0);
            Grid.SetRow(storeHeader, 0);

            gallery = new WrapPanel //wrapPanel for the productsgallery
            {
                Margin = new Thickness(5)
            };
            grid.Children.Add(gallery);
            Grid.SetColumn(gallery, 0);
            Grid.SetRow(gallery, 1);

            for (int i = 0; i < products.Count; i++) //loop that creates the shopview
            {
                Grid productViewGrid = new Grid();
                productViewGrid.RowDefinitions.Add(new RowDefinition());
                productViewGrid.RowDefinitions.Add(new RowDefinition());
                productViewGrid.RowDefinitions.Add(new RowDefinition());
                productViewGrid.ColumnDefinitions.Add(new ColumnDefinition());

                gallery.Children.Add(productViewGrid);

                ImageSource source = new BitmapImage(new Uri(@$"Photos\PS5{i+1}.png", UriKind.Relative));
                Image image = new Image
                {
                    Source = source,
                    Width = 150,
                    Height = 170,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

                pixButton = new Button
                {
                    Content = image,
                    Margin = new Thickness(5),
                    Height = 140,
                    Width = 150,
                    Background = Brushes.Gray,
                    Tag = products[i]
                };
                
                productViewGrid.Children.Add(pixButton);
                Grid.SetRow(productViewGrid, 0);

                productInfo = new Label
                {
                    Content = $"{products[i].Name} - {products[i].Description}\nPris: {products[i].Price}kr",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                productViewGrid.Children.Add(productInfo);
                Grid.SetRow(productInfo, 1);

                addToCart = new Button
                {
                    Content = "Add to Cart",
                    FontSize = 12,
                    Width = 100,
                    FontWeight = FontWeights.Bold,
                    FontStyle = FontStyles.Italic,
                    Tag = products[i]
                };
                productViewGrid.Children.Add(addToCart);
                Grid.SetRow(addToCart, 2);

                pixButton.Click += PixButton_Click;
                addToCart.Click += AddToCart_Click;
            }
            #endregion

            #region Cart
            Label cartHeader = new Label
            {
                Content = "Your Cart",
                FontSize = 25,
                FontWeight = FontWeights.Bold,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(cartHeader);
            Grid.SetColumn(cartHeader, 1);

            Grid cartGrid = new Grid();
            cartGrid.Margin = new Thickness(5);
            cartGrid.ShowGridLines = true;
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.Children.Add(cartGrid);
            Grid.SetColumn(cartGrid, 1);
            Grid.SetRow(cartGrid, 1);

            Label headerName = new Label
            {
                Content = "NAME",
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue
            };
            cartGrid.Children.Add(headerName);
            Grid.SetColumn(headerName, 0);
            Grid.SetRow(headerName, 0);

            Label headerDescription = new Label
            {
                Content = "DESCRIPTION",
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue
            };
            cartGrid.Children.Add(headerDescription);
            Grid.SetColumn(headerDescription, 1);
            Grid.SetRow(headerDescription, 0);

            Label headerPrice = new Label
            {
                Content = "QUANTITY",
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue
            };
            cartGrid.Children.Add(headerPrice);
            Grid.SetColumn(headerPrice, 2);
            Grid.SetRow(headerPrice, 0);

            Label headerQuantity = new Label
            {
                Content = "PRICE",
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue
            };
            cartGrid.Children.Add(headerQuantity);
            Grid.SetColumn(headerQuantity, 3);
            Grid.SetColumnSpan(headerQuantity, 2);
            Grid.SetRow(headerQuantity, 0);

            cartPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
                
            };
            cartGrid.Children.Add(cartPanel);
            Grid.SetColumnSpan(cartPanel, 5);
            Grid.SetRow(cartPanel, 1);

            Label discountCodeLabel = new Label
            {
                Content = "Got a code? Enter it here.",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            cartGrid.Children.Add(discountCodeLabel);
            Grid.SetColumnSpan(discountCodeLabel, 2);
            Grid.SetColumn(discountCodeLabel, 0);
            Grid.SetRow(discountCodeLabel, 2);

            discountCodeBox = new TextBox
            {
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Width = 250,
                HorizontalAlignment = HorizontalAlignment.Left,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(2),
                //CornerRadius = new CornerRadius(15)
            };
            cartGrid.Children.Add(discountCodeBox);
            Grid.SetColumnSpan(discountCodeBox, 3);
            Grid.SetColumn(discountCodeBox, 2);
            Grid.SetRow(discountCodeBox, 2);


            totalCartPriceLabel = new Label
            {
                Content = $"Total Cost: {totalCartPrice}",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            cartGrid.Children.Add(totalCartPriceLabel);
            Grid.SetColumnSpan(totalCartPriceLabel, 3);
            Grid.SetColumn(totalCartPriceLabel, 3);
            Grid.SetRow(totalCartPriceLabel, 3);


            
            #endregion
        }
       
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Product p = (Product)button.Tag;
            MessageBox.Show($"{p.Name} has been added to your cart");

            Cart itemToCart = new Cart(p.Name, p.Description, p.Price, 1);

            //cartItemQuantity = 1;

            //if (!productNamesForComparison.Contains(itemToCart.Name))
            {
                currentCart.Add(itemToCart);
                productNamesForComparison.Add(itemToCart.Name);
                totalCartPrice += itemToCart.Price;
                totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
                CreateCartItemsGrid(itemToCart);
            }


            //else
            {
                //int cartItemsIndexToAffect = productNamesForComparison.IndexOf(itemToCart.Name);
                //Cart cartItemsGridToAffect = (Cart)delete.Tag;
                //int cartItemsIndexToAffect = currentCart.IndexOf(cartItemsGridToAffect);
                //cartItemQuantity += 1;
                //cartPanel.Children.RemoveAt(cartItemsIndexToAffect);
                //Cart correctQuantity = (Cart)quantity.Content;
               //quantity.Content = cartItemQuantity;

               // price.Content = itemToCart.Price * cartItemQuantity;
                //currentCart.Add(itemToCart);
                //totalCartPrice += itemToCart.Price;
                //totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
            }
        }

        private void CreateCartItemsGrid(Cart itemToCart)
        {
            cartItemsGrid = new Grid();
            cartItemsGrid.ShowGridLines = true;

            cartItemsGrid.RowDefinitions.Add(new RowDefinition());
            cartItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartItemsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            cartItemsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            cartItemsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Label name = new Label
            {
                Content = itemToCart.Name,
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            cartItemsGrid.Children.Add(name);
            Grid.SetColumn(name, 0);

            Label description = new Label
            {
                Content = itemToCart.Description,
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            cartItemsGrid.Children.Add(description);
            Grid.SetColumn(description, 1);

            price = new Label
            {
                Content = itemToCart.Price,
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Left,
                //Tag = currentCart[counter]
                Tag = itemToCart
            };
            cartItemsGrid.Children.Add(price);
            Grid.SetColumn(price, 3);

            quantity = new Label
            {
                Content = itemToCart.Quantity,
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Left,
                //Tag = currentCart[counter]
                Tag = itemToCart
            };
            cartItemsGrid.Children.Add(quantity);
            Grid.SetColumn(quantity, 2);

            addCartItemQuantity = new Button
            {
                Content = "+",
                Width = 25,
                //Tag = currentCart[counter]
                Tag = itemToCart
            };
            cartItemsGrid.Children.Add(addCartItemQuantity);
            Grid.SetColumn(addCartItemQuantity, 4);

            addCartItemQuantity.Click += AddCartItemQuantity_Click;

            removeCartItemQuantity = new Button
            {
                Content = "-",
                Width = 25,
                //Tag = currentCart[counter]
                Tag = itemToCart

            };
            cartItemsGrid.Children.Add(removeCartItemQuantity);
            Grid.SetColumn(removeCartItemQuantity, 5);

            removeCartItemQuantity.Click += RemoveCartItemQuantity_Click;

            ImageSource source = new BitmapImage(new Uri(@$"Photos\Trashcan.png", UriKind.Relative));
            Image trashcan = new Image
            {
                Source = source,
                Width = 25,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            RenderOptions.SetBitmapScalingMode(trashcan, BitmapScalingMode.HighQuality);

            deleteCartItem = new Button
            {
                Content = trashcan,
                Tag = itemToCart
            };
            cartItemsGrid.Children.Add(deleteCartItem);
            Grid.SetColumn(deleteCartItem, 6);

            deleteCartItem.Click += DeleteCartItem_Click;

            cartPanel.Children.Add(cartItemsGrid);
            //counter++;
        }

        private void DeleteCartItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Cart cartItemToDelete = (Cart)button.Tag;

            int indexToDelete = currentCart.IndexOf(cartItemToDelete);

            currentCart.RemoveAt(indexToDelete);

            cartPanel.Children.RemoveAt(indexToDelete);

            totalCartPrice = totalCartPrice - cartItemToDelete.Price;
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
        }

        private void RemoveCartItemQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Cart cartItem2 = (Cart)button.Tag;
            MessageBox.Show(cartItem2.Name);
            int indexToChange = currentCart.IndexOf(cartItem2);

            totalCartPrice -= cartItem2.Price / cartItem2.Quantity;
            cartItem2.Price -= cartItem2.Price / cartItem2.Quantity;
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
            price.Content = cartItem2.Price;
            cartItem2.Quantity -= 1;
            quantity.Content = cartItem2.Quantity;
            
            foreach (var item in currentCart)
            {
                MessageBox.Show(price.Tag.ToString());
            }
        }

        private void AddCartItemQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Cart cartItem = (Cart)button.Tag;
            totalCartPrice += cartItem.Price / cartItem.Quantity;
            cartItem.Price += cartItem.Price / cartItem.Quantity;
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
            price.Content = cartItem.Price;
            cartItem.Quantity += 1;
            quantity.Content = cartItem.Quantity;
        }

        private void PixButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Product p = (Product)button.Tag;
            string message = $"{p.Name} - {p.Description}\nPris: {p.Price}kr";
            MessageBox.Show($"You are viewing {message}. To buy this product just add it to your cart.");
        }

        private void LoadProductList()
        {
            string pathPRoducts = @"c:\Windows\Temp\VS_PS5products.txt";
            string[] lines = File.ReadAllLines(pathPRoducts);
            foreach (string line in lines)
            {
                string[] splitlines = line.Split(';');
                var createProduct = new Product(splitlines[0], splitlines[1], splitlines[2], decimal.Parse(splitlines[3]));
                products.Add(createProduct);
            }
        }
    }
}
