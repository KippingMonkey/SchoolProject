using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFStore
{
    public partial class MainWindow : Window
    {
        public class Product //Data for products
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }

            public Product(string id, string name, string description, decimal price)
            {
                ID = id;
                Name = name;
                Description = description;
                Price = price;
            }
        }
        public class CartObject //Data for cartItems and cart
        {
            //public string ID;
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal PriceEach { get; set; }

            public CartObject(string name, string description, decimal price, int quantity, decimal priceEach)
            {
                //ID = id;
                Name = name;
                Description = description;
                Price = price;
                Quantity = quantity;
                PriceEach = priceEach;  
               
            }
        }
        //instance variables
        public List<Product> products = new List<Product>(); //used in STORE
        public List<CartObject> carts = new List<CartObject>(); //used in CART (to save carts)
        public List<CartObject> currentCart = new List<CartObject>(); //used in CART (contains items for current cart)
        public WrapPanel gallery; //used in STORE (shows productgallery)
        public Button pixButton, addToCart, deleteCartItem, addCartItemQuantity, removeCartItemQuantity; //pixbutton and addToCart in STORE, others in CART
        public Button loadSavedCarts, saveCurrentCart, clearCurrentCart, checkout; //final cartbuttons
        public Label productInfo, quantity, price, totalCartPriceLabel ; //used in STORE
        public Grid cartItemsGrid; //used in CART. Grid that build the pseudo-datagrid of the cart
        public StackPanel cartPanel; //used in CART. Container for above grids.
        public List<string> productNamesForComparison = new List<string>();
        public decimal totalCartPrice = 0;
        public TextBox discountCodeBox;
        public string cartPath = @"c:\Windows\Temp\VS_PS5Cart.txt";
        public string codeUsed;
        public List<Label> cartItemQuantiyLabelList = new List<Label>();
        public List<Label> cartItemPriceLabelList = new List<Label>();
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

            discountCodeBox.KeyDown += DiscountCodeBox_KeyDown;


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

            loadSavedCarts = new Button
            {
                Content = "Load Cart",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cartGrid.Children.Add(loadSavedCarts);
            Grid.SetColumn(loadSavedCarts, 0);
            Grid.SetRow(loadSavedCarts, 4);

            loadSavedCarts.Click += LoadSavedCarts_Click;

            saveCurrentCart = new Button
            {
                Content = "Save Cart",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cartGrid.Children.Add(saveCurrentCart);
            Grid.SetColumn(saveCurrentCart, 1);
            Grid.SetRow(saveCurrentCart, 4);

            saveCurrentCart.Click += SaveCurrentCart_Click;

            clearCurrentCart = new Button
            {
                Content = "Clear Cart",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cartGrid.Children.Add(clearCurrentCart);
            Grid.SetColumn(clearCurrentCart, 2);
            Grid.SetRow(clearCurrentCart, 4);

            clearCurrentCart.Click += ClearCurrentCart_Click;

            checkout = new Button
            {
                Content = "CHECKOUT",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Width = 150,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cartGrid.Children.Add(checkout);
            Grid.SetColumn(checkout, 3);
            Grid.SetColumnSpan(checkout, 2);
            Grid.SetRow(checkout, 4);

            checkout.Click += Checkout_Click;




            #endregion
        }

        private void DiscountCodeBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string codesPath = @"c:\Windows\Temp\VS_PS5DiscountCodes.txt";
            string[] codes = File.ReadAllLines(codesPath);
            TextBox txb = (TextBox)sender;

            if (e.Key == System.Windows.Input.Key.Enter && sender is TextBox)
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    if (txb.Text.ToLower() == codes[0].ToLower())
                    {
                        CartObject codeToCart = new CartObject("Good Deal", "10% Off!", totalCartPrice * (decimal)0.9 - totalCartPrice, 1, 0);
                        currentCart.Add(codeToCart);
                        CreateCartItemsGrid(codeToCart);
                        totalCartPriceLabel.Content = totalCartPrice * (decimal)0.9;
                        discountCodeBox.IsReadOnly = true;
                        codeUsed = codes[0];
                        break;
                    }
                    else if (txb.Text.ToLower() == codes[1].ToLower())
                    {
                        CartObject codeToCart = new CartObject("Great Deal", "25% Off!", totalCartPrice * (decimal)0.75 - totalCartPrice, 1, 0);
                        currentCart.Add(codeToCart);
                        CreateCartItemsGrid(codeToCart);
                        totalCartPriceLabel.Content = totalCartPrice * (decimal)0.75;
                        discountCodeBox.IsReadOnly = true;
                        codeUsed = codes[1];
                        break;
                    }
                    else if (txb.Text.ToLower() == codes[2].ToLower())
                    {
                        CartObject codeToCart = new CartObject("Awsome Deal", "50% Off!", totalCartPrice * (decimal)0.5 - totalCartPrice, 1, 0);
                        currentCart.Add(codeToCart);
                        CreateCartItemsGrid(codeToCart);
                        totalCartPriceLabel.Content = totalCartPrice * (decimal)0.5;
                        discountCodeBox.IsReadOnly = true;
                        codeUsed = codes[2];
                        break;
                    }
                    else if (i == codes.Length -1)
                    {
                        MessageBox.Show("I am sorry. That code was not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        discountCodeBox.Text = "";
                    }
                }
               
            }
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder receipt = new StringBuilder("NAME\t\tDESCRIPTION\tQTY\tEACH\tTOTAL\n", 100);
            foreach (var item in currentCart)
            {
                receipt.AppendLine($"{item.Name, -20}\t{item.Description, -15}\t{item.Quantity, 3}\t{item.PriceEach, -5}\t{item.Price}\n");
            }
            receipt.AppendLine("=================================================\n");
            receipt.AppendLine($"\nTotal amount charged: {totalCartPrice,-10}\n");
            receipt.AppendLine($"Code used: {codeUsed}");
            MessageBox.Show(receipt.ToString(), "RECEIPT", MessageBoxButton.OK);
            
        }

        private void LoadSavedCarts_Click(object sender, RoutedEventArgs e)
        {
            currentCart.Clear();
            cartPanel.Children.Clear();
            string[] lines = File.ReadAllLines(cartPath);
            foreach  (string line in lines)
            {
                string[] splitlines = line.Split(';');
                var createCartItem = new CartObject(splitlines[0], splitlines[1], decimal.Parse(splitlines[3]), int.Parse(splitlines[2]), decimal.Parse(splitlines[4]));
                currentCart.Add(createCartItem);
                codeUsed = splitlines[5];
            }
            totalCartPrice = 0;
            foreach (var item in currentCart)
            {
                CreateCartItemsGrid(item);
                totalCartPrice += item.Price;
                if (item.Description == "10% Off!" || item.Description == "25% Off!" || item.Description == "50% Off!")
                {
                    discountCodeBox.IsReadOnly = true;
                }
            }
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
        }

        private void SaveCurrentCart_Click(object sender, RoutedEventArgs e)
        {
            string savedCartTextfile = "";
            foreach (CartObject item in currentCart)
            {
                savedCartTextfile += $"{item.Name};{item.Description};{item.Quantity};{item.Price};{item.PriceEach};{codeUsed}\n";
            }
            File.WriteAllText(cartPath, savedCartTextfile);
            MessageBox.Show("Your Cart has been saved");
            currentCart.Clear();
            cartPanel.Children.Clear();
            totalCartPriceLabel.Content = $"Total Cost: " + 0;
            totalCartPrice = 0;
            discountCodeBox.Text = "";

        }

        private void ClearCurrentCart_Click(object sender, RoutedEventArgs e)
        {
            currentCart.Clear();
            cartPanel.Children.Clear();
            totalCartPriceLabel.Content = $"Total Cost: " + 0;
            discountCodeBox.Text = "";
            totalCartPrice = 0;
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Product p = (Product)button.Tag;
            MessageBox.Show($"{p.Name} has been added to your cart");

            CartObject itemToCart = new CartObject(p.Name, p.Description, p.Price, 1, p.Price);

            if (!productNamesForComparison.Contains(itemToCart.Name))
            {
                currentCart.Add(itemToCart);
                productNamesForComparison.Add(itemToCart.Name);
                totalCartPrice += itemToCart.Price;
                totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
                CreateCartItemsGrid(itemToCart);
            }

            else
            {
                int cartItemsIndexToAffect = productNamesForComparison.IndexOf(itemToCart.Name);

                //Button button = (Button)sender;
                //CartObject cartItem = (CartObject)butitemToCart
                int indexToChange = currentCart.IndexOf(itemToCart);

                totalCartPrice += itemToCart.Price / itemToCart.Quantity;
                itemToCart.Price += itemToCart.Price / itemToCart.Quantity;
                totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
                cartItemPriceLabelList[cartItemsIndexToAffect].Content = itemToCart.Price;
                itemToCart.Quantity += 1;
                cartItemQuantiyLabelList[cartItemsIndexToAffect].Content = itemToCart.Quantity;

            }
        }

        private void CreateCartItemsGrid(CartObject itemToCart)
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
            cartItemPriceLabelList.Add(price);

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
            cartItemQuantiyLabelList.Add(quantity);

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
                Tag = itemToCart,
                
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
            CartObject cartItemToDelete = (CartObject)button.Tag;

            int indexToDelete = currentCart.IndexOf(cartItemToDelete);

            currentCart.RemoveAt(indexToDelete);

            cartPanel.Children.RemoveAt(indexToDelete);

            totalCartPrice = totalCartPrice - cartItemToDelete.Price;
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
        }

        private void RemoveCartItemQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CartObject cartItem2 = (CartObject)button.Tag;
            int indexToChange = currentCart.IndexOf(cartItem2);

            totalCartPrice -= cartItem2.Price / cartItem2.Quantity;
            cartItem2.Price -= cartItem2.Price / cartItem2.Quantity;
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
            cartItemPriceLabelList[indexToChange].Content = cartItem2.Price;
            cartItem2.Quantity -= 1;
            cartItemQuantiyLabelList[indexToChange].Content = cartItem2.Quantity;

        }

        private void AddCartItemQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CartObject cartItem = (CartObject)button.Tag;
            int indexToChange = currentCart.IndexOf(cartItem);

            totalCartPrice += cartItem.Price / cartItem.Quantity;
            cartItem.Price += cartItem.Price / cartItem.Quantity;
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
            cartItemPriceLabelList[indexToChange].Content = cartItem.Price;
            cartItem.Quantity += 1;
            cartItemQuantiyLabelList[indexToChange].Content = cartItem.Quantity;
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
            string pathProducts = @"c:\Windows\Temp\VS_PS5products.txt";
            string[] lines = File.ReadAllLines(pathProducts);
            foreach (string line in lines)
            {
                string[] splitlines = line.Split(';');
                var createProduct = new Product(splitlines[0], splitlines[1], splitlines[2], decimal.Parse(splitlines[3]));
                products.Add(createProduct);
            }
        }
    }
}
