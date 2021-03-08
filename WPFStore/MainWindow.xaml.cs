using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            public bool IsActive { get; set; }


            public Product(string id, string name, string description, decimal price, bool isActive)
            {
                ID = id;
                Name = name;
                Description = description;
                Price = price;
                IsActive = isActive;
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
                Name = name;
                Description = description;
                Price = price;
                Quantity = quantity;
                PriceEach = priceEach;  
            }
        }
        //instance variables to do with Product class and STORE
        public Button productImageButton;
        public Button addItemToCart;
        public Label productInfo;
        public List<Product> products = new List<Product>();
        public List<Label> cartItemQuantiyLabelList = new List<Label>(); //keeps track of quantity labels in the cart
        public List<Label> cartItemPriceLabelList = new List<Label>(); //keeps track of price labels in the cart
        public WrapPanel productGalleryPanel;

        //instance variables to do with CartObject class and CART
        public Button deleteCartItem, addCartItemQuantity, removeCartItemQuantity;
        public Button loadSavedCarts, saveCurrentCart, clearCurrentCart, checkout;
        public decimal totalCartPrice = 0;
        public Grid cartItemsGrid; //used in CART. Grid that build the pseudo-datagrid of the cart
        public Label quantity, price, totalCartPriceLabel; //used in cartGrid
        public List<CartObject> currentCartItemList = new List<CartObject>();
        public List<CartObject> discountCodesList = new List<CartObject>();
        public List<string> productNamesForComparisonList = new List<string>(); //used in if/else to see if the cart already contains the product
        public StackPanel cartPanel; //used in CART. Container for above grids.
        public string cartPathSaveLoad = @"c:\Windows\Temp\VS_PS5Cart.txt"; //used for loading and saving the current cart
        public string codeUsed = "";
        public TextBox discountCodeBox;
      
        

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

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
            grid.ShowGridLines = false; //change to true if you want to view gridlines

            Closing += MainWindow_Closing;

            #region STORE 
            //region that encompasses the store-part of the GUI

            LoadProductList(@"VS_PS5products.txt");
            //@"c:\Windows\Temp\VS_PS5products.txt"

            Label storeHeader = new Label
            {
                Content = "Welcome to the PS5 Store",
                FontSize = 25,
                FontWeight = FontWeights.Bold,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(storeHeader);
            Grid.SetColumn(storeHeader, 0);
            Grid.SetRow(storeHeader, 0);

            CreateProductGallery(grid);
           
            #endregion

            #region CART
            //region that encompasses the cart part of the GUI
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

            CreateCartViewGrid(grid);
            LoadDiscountCodes(@"VS_PS5DiscountCodes.txt");

            #endregion
        }
        //METHODS AND EVENT HANDLERS USED BY STORE
        // loads all available products from a file
        private void LoadProductList(string path)
        {
            string tempPath = @"c:\Windows\Temp\VS_PS5products.txt";
            if (File.Exists(tempPath))
            {
                string[] lines = File.ReadAllLines(tempPath);
                foreach (string line in lines)
                {
                    string[] splitlines = line.Split(';');
                    var createProduct = new Product(splitlines[0], splitlines[1], splitlines[2], decimal.Parse(splitlines[3]), bool.Parse(splitlines[4]));
                    products.Add(createProduct);
                }
            }
            else 
            {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    string[] splitlines = line.Split(';');
                    var createProduct = new Product(splitlines[0], splitlines[1], splitlines[2], decimal.Parse(splitlines[3]), bool.Parse(splitlines[4]));
                    products.Add(createProduct);
                }
            }
        }

        // creates the image gallery for the store
        private void CreateProductGallery(Grid grid)
        {
            productGalleryPanel = new WrapPanel //wrapPanel for the productsgallery
            {
                Margin = new Thickness(5)
            };
            grid.Children.Add(productGalleryPanel);
            Grid.SetColumn(productGalleryPanel, 0);
            Grid.SetRow(productGalleryPanel, 1);

            for (int i = 0; i < products.Count; i++) //loop that creates the shopview
            {
                Grid productViewGrid = new Grid();
                productViewGrid.RowDefinitions.Add(new RowDefinition());
                productViewGrid.RowDefinitions.Add(new RowDefinition());
                productViewGrid.RowDefinitions.Add(new RowDefinition());
                productViewGrid.ColumnDefinitions.Add(new ColumnDefinition());

                productGalleryPanel.Children.Add(productViewGrid);

                Image image = LoadProductImage(i);

                productImageButton = new Button
                {
                    Content = image,
                    Margin = new Thickness(5),
                    Height = 140,
                    Width = 150,
                    Background = Brushes.Gray,
                    Tag = products[i]
                };

                productViewGrid.Children.Add(productImageButton);
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

                addItemToCart = new Button
                {
                    Content = "Add to Cart",
                    FontSize = 12,
                    Width = 100,
                    FontWeight = FontWeights.Bold,
                    FontStyle = FontStyles.Italic,
                    Tag = products[i]
                };
                productViewGrid.Children.Add(addItemToCart);
                Grid.SetRow(addItemToCart, 2);

                productImageButton.Click += ProductImageButton_Click;
                addItemToCart.Click += AddToCart_Click;
            }
        }

        private Image LoadProductImage(int i)
        {
            if (!Directory.Exists(@"c:\Windows\Temp\VS_PS5Photos"))
            {
                ImageSource source = new BitmapImage(new Uri(@$"Photos\{products[i].ID}.png", UriKind.Relative));
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

                return image;
            }
            else
            {
                ImageSource source = new BitmapImage(new Uri($@"c:\Windows\Temp\VS_PS5Photos\{products[i].ID}.png", UriKind.RelativeOrAbsolute));
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

                return image;
            }
           
        }

        // let's the user see information about product when the product picture is clicked
        private void ProductImageButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Product p = (Product)button.Tag;
            string message = $"{p.Name} - {p.Description}\nPris: {p.Price}kr";
            MessageBox.Show($"You are viewing {message}. To buy this product just add it to your cart.");
        }

        // adds item from store to the current cart
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Product p = (Product)button.Tag;
            MessageBox.Show($"{p.Name} has been added to your cart");

            CartObject cartItemAdded = new CartObject(p.Name, p.Description, p.Price, 1, p.Price);

            if (!productNamesForComparisonList.Contains(cartItemAdded.Name)) //If the product doesn't exist - add it to cart
            {
                currentCartItemList.Add(cartItemAdded);
                productNamesForComparisonList.Add(cartItemAdded.Name);
                totalCartPrice += cartItemAdded.Price;
                totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
                CreateCartItemsGrid(cartItemAdded);
            }
            else // if it does adjust quantity, price and totalprice in cart
            {
                int indexToChange = productNamesForComparisonList.IndexOf(cartItemAdded.Name);
                cartItemAdded = currentCartItemList[indexToChange];
                UpdateItemQuantityPriceAndTotalCartPrice(cartItemAdded, indexToChange);
            }
        }
       
        // Creates a textfile in TEMP with the productlist
        private void SaveProductListToFile()
        {
            string saveProductString = "";
            foreach (Product p in products)
            {
                saveProductString += $"{p.ID};{p.Name};{p.Description};{p.Price};{p.IsActive}\n";
            }
            File.WriteAllText(@"c:\Windows\Temp\VS_PS5products.txt", saveProductString);
        }

        // Saves photos in project to a new folder in TEMP
        private void SavePhotosToFile()
        {
            if (!Directory.Exists(@"c:\Windows\Temp\VS_PS5Photos"))
            {
                Directory.CreateDirectory(@"c:\Windows\Temp\VS_PS5Photos");
                string[] fileEntries = Directory.GetFiles(@"Photos\");
                foreach (string source in fileEntries)
                {
                    string fileName = source.Substring(7);
                    string destination = $@"c:\Windows\Temp\VS_PS5Photos\{fileName}";
                    File.Copy(source, destination);
                }
            }
            else { }


        }

        //====================================================================================================

        // METHOD USED BY BOTH STORE AND CART
        private void UpdateItemQuantityPriceAndTotalCartPrice(CartObject cartItemAdded, int indexToChange)
        {
            totalCartPrice = CalculateTotalCartPriceWhenAddingProduct(cartItemAdded, totalCartPrice);
            cartItemAdded.Price += cartItemAdded.PriceEach;
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
            cartItemPriceLabelList[indexToChange].Content = cartItemAdded.Price;
            cartItemAdded.Quantity += 1;
            cartItemQuantiyLabelList[indexToChange].Content = cartItemAdded.Quantity;
        }
        // broken out for testing
        public static decimal CalculateTotalCartPriceWhenAddingProduct(CartObject cartItemAdded, decimal totalCartPrice)
        {
            totalCartPrice += cartItemAdded.PriceEach;
            return totalCartPrice;
        }

        //Saves Data to files in TEMP when closing the app
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveProductListToFile();
            SaveCodeListToFile();
            SavePhotosToFile();
        }

        //=====================================================================================================

        //METHODS AND EVENTS USED BY CART
        // creates the cart view
        private void CreateCartViewGrid(Grid grid)
        {
            Grid cartViewGrid = new Grid();
            cartViewGrid.Margin = new Thickness(5);
            cartViewGrid.ShowGridLines = false;
            cartViewGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartViewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            cartViewGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartViewGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartViewGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartViewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
            cartViewGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartViewGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartViewGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartViewGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartViewGrid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.Children.Add(cartViewGrid);
            Grid.SetColumn(cartViewGrid, 1);
            Grid.SetRow(cartViewGrid, 1);

            Label headerName = new Label
            {
                Content = "NAME",
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue
            };
            cartViewGrid.Children.Add(headerName);
            Grid.SetColumn(headerName, 0);
            Grid.SetRow(headerName, 0);

            Label headerDescription = new Label
            {
                Content = "DESCRIPTION",
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue
            };
            cartViewGrid.Children.Add(headerDescription);
            Grid.SetColumn(headerDescription, 1);
            Grid.SetRow(headerDescription, 0);

            Label headerPrice = new Label
            {
                Content = "QUANTITY",
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue
            };
            cartViewGrid.Children.Add(headerPrice);
            Grid.SetColumn(headerPrice, 2);
            Grid.SetRow(headerPrice, 0);

            Label headerQuantity = new Label
            {
                Content = "PRICE",
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue
            };
            cartViewGrid.Children.Add(headerQuantity);
            Grid.SetColumn(headerQuantity, 3);
            Grid.SetColumnSpan(headerQuantity, 2);
            Grid.SetRow(headerQuantity, 0);

            cartPanel = new StackPanel
            {
                Orientation = Orientation.Vertical

            };
            cartViewGrid.Children.Add(cartPanel);
            Grid.SetColumnSpan(cartPanel, 5);
            Grid.SetRow(cartPanel, 1);

            Label discountCodeLabel = new Label
            {
                Content = "Got a code? Enter it here.",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            cartViewGrid.Children.Add(discountCodeLabel);
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
            };
            cartViewGrid.Children.Add(discountCodeBox);
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
            cartViewGrid.Children.Add(totalCartPriceLabel);
            Grid.SetColumnSpan(totalCartPriceLabel, 3);
            Grid.SetColumn(totalCartPriceLabel, 3);
            Grid.SetRow(totalCartPriceLabel, 3);

            loadSavedCarts = new Button
            {
                Content = "Load Cart",
                FontSize = 15,
                Width = 90,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cartViewGrid.Children.Add(loadSavedCarts);
            Grid.SetColumn(loadSavedCarts, 0);
            Grid.SetRow(loadSavedCarts, 4);

            loadSavedCarts.Click += LoadSavedCarts_Click;

            saveCurrentCart = new Button
            {
                Content = "Save Cart",
                FontSize = 15,
                Width = 90,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cartViewGrid.Children.Add(saveCurrentCart);
            Grid.SetColumn(saveCurrentCart, 1);
            Grid.SetRow(saveCurrentCart, 4);

            saveCurrentCart.Click += SaveCurrentCart_Click;

            clearCurrentCart = new Button
            {
                Content = "Clear Cart",
                FontSize = 15,
                Width = 90,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cartViewGrid.Children.Add(clearCurrentCart);
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
            cartViewGrid.Children.Add(checkout);
            Grid.SetColumn(checkout, 3);
            Grid.SetColumnSpan(checkout, 2);
            Grid.SetRow(checkout, 4);

            checkout.Click += Checkout_Click;
        }
        // creates a new grid for the added item and puts it in the stack panel in the cart view grid
        private void CreateCartItemsGrid(CartObject itemToCart)
        {
            cartItemsGrid = new Grid();
            cartItemsGrid.ShowGridLines = false;
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
                Tag = itemToCart
            };
            cartItemsGrid.Children.Add(quantity);
            Grid.SetColumn(quantity, 2);
            cartItemQuantiyLabelList.Add(quantity);

            addCartItemQuantity = new Button
            {
                Content = "+",
                Width = 25,
                Tag = itemToCart
            };
            cartItemsGrid.Children.Add(addCartItemQuantity);
            Grid.SetColumn(addCartItemQuantity, 4);

            addCartItemQuantity.Click += AddCartItemQuantity_Click;

            removeCartItemQuantity = new Button
            {
                Content = "-",
                Width = 25,
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
        }

        // add quantity to a specific item in the current cart
        private void AddCartItemQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CartObject cartItemAdded = (CartObject)button.Tag;
            int indexToChange = currentCartItemList.IndexOf(cartItemAdded);
            UpdateItemQuantityPriceAndTotalCartPrice(cartItemAdded, indexToChange);
            //totalCartPrice += cartItemAdded.Price / cartItemAdded.Quantity;
            //cartItemAdded.Price += cartItemAdded.Price / cartItemAdded.Quantity;
            //totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
            //cartItemPriceLabelList[indexToChange].Content = cartItemAdded.Price;
            //cartItemAdded.Quantity += 1;
            //cartItemQuantiyLabelList[indexToChange].Content = cartItemAdded.Quantity;
        }

        // reduces quantity of a specific item in the current cart
        private void RemoveCartItemQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CartObject cartItemSubtracted = (CartObject)button.Tag;
            int indexToChange = currentCartItemList.IndexOf(cartItemSubtracted);

            // avoids the possibility to divide by zero
            if (cartItemSubtracted.Quantity >= 2)
            {
                totalCartPrice -= cartItemSubtracted.PriceEach;
                cartItemSubtracted.Price -= cartItemSubtracted.PriceEach;
                totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
                cartItemPriceLabelList[indexToChange].Content = cartItemSubtracted.Price;
                cartItemSubtracted.Quantity -= 1;
                cartItemQuantiyLabelList[indexToChange].Content = cartItemSubtracted.Quantity;
            }
            else
            {
                DeleteCartItemMethod(cartItemSubtracted, indexToChange);
            }
        }

        // deletes an item from the current cart
        private void DeleteCartItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CartObject cartItemToDelete = (CartObject)button.Tag;

            int indexToDelete = currentCartItemList.IndexOf(cartItemToDelete);

            DeleteCartItemMethod(cartItemToDelete, indexToDelete);

        }

        private void DeleteCartItemMethod(CartObject cartItemToDelete, int indexToDelete)
        {
            currentCartItemList.RemoveAt(indexToDelete);
            productNamesForComparisonList.RemoveAt(indexToDelete);
            cartPanel.Children.RemoveAt(indexToDelete);
            cartItemPriceLabelList.RemoveAt(indexToDelete);
            cartItemQuantiyLabelList.RemoveAt(indexToDelete);
            totalCartPrice = totalCartPrice - cartItemToDelete.Price;
            totalCartPriceLabel.Content = $"Total Cost: {totalCartPrice}";
        }

        public void LoadDiscountCodes(string path)
        {
            string tempPath = @"c:\Windows\Temp\VS_PS5products.txt";
            if (File.Exists(tempPath))
            {
                string[] lines = File.ReadAllLines(tempPath);
                foreach (string line in lines)
                {
                    string[] splitlines = line.Split(';');
                    CartObject createCode = new CartObject(splitlines[0], splitlines[1], decimal.Parse(splitlines[2]), int.Parse(splitlines[3]), decimal.Parse(splitlines[4]));
                    discountCodesList.Add(createCode);
                }
            }
            else if (!File.Exists(tempPath)
            {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    string[] splitlines = line.Split(';');
                    CartObject createCode = new CartObject(splitlines[0], splitlines[1], decimal.Parse(splitlines[2]), int.Parse(splitlines[3]), decimal.Parse(splitlines[4]));
                    discountCodesList.Add(createCode);
                }
            }
            else
            {
                MessageBox.Show("This file does not exist", "File Search Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        //handles actions related to the discount textbox
        private void DiscountCodeBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox txb = (TextBox)sender;

            if (e.Key == System.Windows.Input.Key.Enter && sender is TextBox)
            {
                foreach (CartObject code in discountCodesList)
                {
                    if (txb.Text.ToLower() == code.Name.ToLower())
                    {
                        totalCartPriceLabel.Content = totalCartPrice * code.Price;
                        code.Price = totalCartPrice * code.Price - totalCartPrice;
                        currentCartItemList.Add(code);
                        CreateCartItemsGrid(code);
                        discountCodeBox.IsReadOnly = true;
                        codeUsed = code.Name;
                        break;
                    }
                }
                if (codeUsed == "")
                {
                    MessageBox.Show("I am sorry. That code was not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    discountCodeBox.Text = "";
                }
            }
        }

        //loads saved cart (if there is one)
        private void LoadSavedCarts_Click(object sender, RoutedEventArgs e)
        {
            currentCartItemList.Clear();
            cartPanel.Children.Clear();
            string[] lines = File.ReadAllLines(cartPathSaveLoad);
            foreach  (string line in lines)
            {
                string[] splitlines = line.Split(';');
                var createCartItem = new CartObject(splitlines[0], splitlines[1], decimal.Parse(splitlines[3]), int.Parse(splitlines[2]), decimal.Parse(splitlines[4]));
                currentCartItemList.Add(createCartItem);
                codeUsed = splitlines[5];
            }
            totalCartPrice = 0;
            foreach (var item in currentCartItemList)
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

        //saves the current cart to file
        private void SaveCurrentCart_Click(object sender, RoutedEventArgs e)
        {
            string savedCartTextfile = "";
            foreach (CartObject item in currentCartItemList)
            {
                savedCartTextfile += $"{item.Name};{item.Description};{item.Quantity};{item.Price};{item.PriceEach};{codeUsed}\n";
            }
            File.WriteAllText(cartPathSaveLoad, savedCartTextfile);
            MessageBox.Show("Your Cart has been saved");
            ClearCart();
        }

        //handles action related to the clear cart button
        private void ClearCurrentCart_Click(object sender, RoutedEventArgs e)
        {
            ClearCart();
        }

        //resets the whole cart
        private void ClearCart()
        {
            productNamesForComparisonList.Clear();
            cartItemPriceLabelList.Clear();
            cartItemQuantiyLabelList.Clear();
            currentCartItemList.Clear();
            cartPanel.Children.Clear();
            totalCartPriceLabel.Content = $"Total Cost: " + 0;
            discountCodeBox.Text = "";
            totalCartPrice = 0;
            codeUsed = "";
            discountCodeBox.IsReadOnly = false;
        }

        //bring out the receipt and resets the cart
        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            decimal sum = 0;
            StringBuilder receipt = new StringBuilder("NAME\t\tDESCRIPTION\tQTY\tEACH\tTOTAL\n", 100);
            foreach (var item in currentCartItemList)
            {
                receipt.AppendLine($"{item.Name,-20}\t{item.Description,-15}\t{item.Quantity,3}\t{item.PriceEach,-5}\t{item.Price}\n");
                sum += item.Price;
            }
            receipt.AppendLine("=================================================\n");
            receipt.AppendLine($"\nTotal amount charged: {sum,-10}\n");
            receipt.AppendLine($"Code used: {codeUsed}");
            MessageBox.Show(receipt.ToString(), "RECEIPT", MessageBoxButton.OK);
            ClearCart();
        }

        // Saves list with discount codes to a new file in TEMP
        private void SaveCodeListToFile()
        {
            string saveCodeString = "";
            foreach (CartObject c in discountCodesList)
            {
                saveCodeString += $"{c.Name};{c.Description};{c.Price};{c.Quantity};{c.PriceEach}\n";
            }
            File.WriteAllText(@"c:\Windows\Temp\VS_PS5DiscountCodes.txt", saveCodeString);
        }

        //Just for trying out testing. No other function
        public static int AddNumbers(int a, int b)
        {
            int result = a + b;
            return result;

        }
    }
}
