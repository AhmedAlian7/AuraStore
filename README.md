# AURA Store - E-Commerce Platform

A comprehensive, modern e-commerce platform built with ASP.NET Core MVC, featuring a complete online shopping experience with advanced features for both customers and administrators.

## 🚀 Project Overview

AURA Store is a full-featured e-commerce solution that provides a seamless shopping experience with modern design, secure payment processing, and comprehensive admin management capabilities. The platform supports multiple user roles, advanced product management, and integrated third-party services.

## ✨ Key Features

### 🛍️ Customer Features
- **Product Catalog**: Browse products with advanced filtering, search, and categorization
- **Shopping Cart**: Add/remove items, quantity management, and cart persistence
- **Wishlist**: Save favorite products for later purchase
- **User Authentication**: Secure registration/login with Google and Facebook OAuth
- **Order Management**: Complete order tracking and history
- **Product Reviews & Ratings**: 5-star rating system with detailed reviews
- **Promo Codes**: Apply discount codes with percentage or fixed amount discounts
- **Product Notifications**: Get notified when out-of-stock products become available

### 👨‍💼 Admin Features
- **Dashboard**: Comprehensive analytics and overview
- **Product Management**: CRUD operations for products with image upload
- **Category Management**: Organize products into categories
- **Order Management**: Process orders, update status, and send notifications
- **User Management**: Manage customer accounts and permissions
- **Promo Code Management**: Create and manage discount codes
- **Inventory Tracking**: Monitor stock levels and product availability

### 🔧 Technical Features
- **Payment Processing**: Stripe integration for secure payments
- **Email Notifications**: SMTP-based email system for order updates
- **Image Management**: Cloudinary integration for product image storage
- **Role-Based Security**: Admin and Customer role separation
- **Database Management**: Entity Framework Core with SQL Server
- **Exception Handling**: Global exception handling with custom filters

## 🏗️ Architecture

The project follows a clean, layered architecture pattern:

```
E-Commerce-03/
├── E-Commerce.Web/           # Presentation Layer (MVC)
├── E-Commerce.Business/      # Business Logic Layer
└── E-Commerce.DataAccess/    # Data Access Layer
```

### Layer Responsibilities

- **Web Layer**: Controllers, Views, Areas (Admin/Customer/Authentication), Static files
- **Business Layer**: Services, ViewModels, AutoMapper profiles, External service integrations
- **Data Access Layer**: Entities, Repositories, DbContext, Migrations, Seeding

## 🛠️ Technology Stack

### Backend Technologies
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core MVC** - Web application framework
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server** - Primary database
- **AutoMapper 12.0** - Object-to-object mapping
- **Stripe.NET 48.4** - Payment processing
- **Cloudinary** - Image storage and management

### Frontend Technologies
- **Bootstrap 5** - CSS framework
- **jQuery** - JavaScript library
- **Font Awesome** - Icon library
- **Custom CSS** - Modern, responsive styling
- **AJAX** - Asynchronous operations

### Authentication & Security
- **ASP.NET Core Identity** - User management and authentication
- **Google OAuth** - Social login integration
- **Facebook OAuth** - Social login integration
- **Role-based Authorization** - Admin/Customer access control

### External Services
- **Stripe** - Payment processing
- **Cloudinary** - Image storage and optimization
- **Gmail SMTP** - Email notifications

## 📊 Database Schema

The application uses a comprehensive database schema with the following main entities:

- **Users** (ApplicationUser) - Customer and admin accounts
- **Products** - Product catalog with images and categories
- **Categories** - Product categorization
- **Carts & CartItems** - Shopping cart functionality
- **Orders & OrderItems** - Order management
- **Reviews** - Product reviews and ratings
- **PromoCodes** - Discount code system
- **WishlistItems** - Customer wishlists
- **ProductNotifications** - Stock notification system

## 📱 Features in Detail

### Customer Experience
- **Homepage**: Featured products, trending items, customer reviews
- **Product Browsing**: Category filtering, search functionality, pagination
- **Product Details**: High-quality images, detailed descriptions, reviews
- **Shopping Cart**: Real-time updates, quantity management, promo code application
- **Checkout**: Secure payment processing with Stripe
- **Order Tracking**: Complete order history and status updates
- **User Profile**: Account management, order history, wishlist

### Admin Experience
- **Dashboard**: Sales analytics, order statistics, user metrics
- **Product Management**: Add/edit products, manage categories, upload images
- **Order Processing**: View orders, update status, send notifications
- **User Management**: Customer accounts, role management
- **Promo Code System**: Create discount codes with various conditions

## 🔒 Security Features

- **Authentication**: Secure user registration and login
- **Authorization**: Role-based access control
- **Data Protection**: Entity Framework Core security
- **Payment Security**: Stripe's secure payment processing
- **Input Validation**: Model validation and sanitization
- **CSRF Protection**: Anti-forgery tokens

## 📈 Performance Optimizations

- **Pagination**: Efficient data loading for large datasets
- **Image Optimization**: Cloudinary integration for optimized images
- **Database Indexing**: Optimized queries with proper indexing
- **Lazy Loading**: Efficient data loading strategies

## 📝 API Documentation

The application includes API endpoints for:
- Product management

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 👥 Support

For support and questions:
- Create an issue in the repository
- Contact the development team

## 🔮 Future Enhancements

- **Recently Viewed Products**: Track and display recently browsed items
- **Advanced Notification System**: Real-time notifications
- **Analytics Dashboard**: Advanced reporting and analytics
- **Advanced Search**: Elasticsearch integration
- **Recommendation Engine**: AI-powered product recommendations

---

**AURA Store** - Your complete e-commerce solution built with modern technologies and best practices.