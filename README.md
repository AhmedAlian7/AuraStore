# AURA Store - E-Commerce Platform

A comprehensive, modern e-commerce platform built with ASP.NET Core MVC, featuring a complete online shopping experience with advanced features for both customers and administrators.

<div align="center">

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-blue.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-9.0-green.svg)](https://docs.microsoft.com/en-us/ef/)
[![C#](https://img.shields.io/badge/C%23-12.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-blue.svg)](https://www.microsoft.com/en-us/sql-server)
[![Stripe](https://img.shields.io/badge/Stripe-Payment-635bff.svg?logo=stripe&logoColor=white)](https://stripe.com)
[![Cloudinary](https://img.shields.io/badge/Cloudinary-Images-3448c5.svg?logo=cloudinary&logoColor=white)](https://cloudinary.com)
[![Google OAuth](https://img.shields.io/badge/Google-OAuth-4285f4.svg?logo=google&logoColor=white)](https://developers.google.com/identity/protocols/oauth2)
[![Facebook OAuth](https://img.shields.io/badge/Facebook-OAuth-1877f2.svg?logo=facebook&logoColor=white)](https://developers.facebook.com/docs/facebook-login/)
[![Gmail SMTP](https://img.shields.io/badge/Gmail-SMTP-ea4335.svg?logo=gmail&logoColor=white)](https://developers.google.com/gmail/smtp/reference)

</div>

## 📋 Table of Contents

- [Project Overview](#-project-overview)
- [Key Features](#-key-features)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Database Schema](#-database-schema)
- [Security Features](#-security-features)
- [Performance Optimizations](#-performance-optimizations)
- [Contributing](#-contributing)

## 🚀 Project Overview

AURA Store is a full-featured e-commerce solution that provides a seamless shopping experience with modern design, secure payment processing, and comprehensive admin management capabilities. The platform supports multiple user roles, advanced product management, and integrated third-party services including Stripe payments and Cloudinary image management.

## ✨ Key Features

### 🛍️ Customer Features
- **Advanced Product Catalog**: Browse products with sophisticated filtering, search, and categorization
- **Smart Shopping Cart**: Add/remove items with quantity management and cart persistence across sessions
- **Wishlist System**: Save favorite products for future purchase with easy management
- **Social Authentication**: Secure registration/login with Google and Facebook OAuth integration
- **Complete Order Management**: Full order tracking, history, and status updates
- **Product Reviews & Ratings**: Comprehensive 5-star rating system with detailed customer reviews
- **Promo Code System**: Apply discount codes with percentage or fixed amount discounts
- **Product Notifications**: Get notified when out-of-stock products become available again

### 👨‍💼 Admin Features
- **Analytics Dashboard**: Comprehensive sales analytics, order statistics, and user metrics
- **Advanced Product Management**: Complete CRUD operations with multi-image upload via Cloudinary
- **Category Management**: Hierarchical product organization and categorization
- **Order Processing Center**: Process orders, update status, and send automated notifications
- **User Management**: Comprehensive customer account and permission management
- **Promo Code Administration**: Create and manage discount codes with various conditions

### 🔧 Technical Features
- **Secure Payment Processing**: Full Stripe integration for PCI-compliant payments
- **Email Notification System**: SMTP-based automated email system for order updates
- **Cloud Image Management**: Cloudinary integration for optimized product image storage
- **Role-Based Security**: Granular Admin and Customer role separation
- **Advanced Database Management**: Entity Framework Core 9.0 with SQL Server
- **Global Exception Handling**: Custom exception filters with comprehensive error logging

## 🏗️ Architecture

The project follows a clean, layered architecture pattern with clear separation of concerns:

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
- **.NET 9.0** - Latest .NET framework with performance improvements
- **ASP.NET Core MVC** - Modern web application framework
- **Entity Framework Core 9.0** - Advanced ORM with Code First approach
- **SQL Server** - Enterprise-grade database
- **Stripe.NET 48.4** - Secure payment processing integration
- **Cloudinary .NET** - Cloud-based image storage and optimization

### Frontend Technologies
- **Bootstrap 5** - Responsive CSS framework with modern components
- **jQuery 3.x** - Enhanced JavaScript functionality
- **Font Awesome** - Comprehensive icon library
- **Custom CSS** - Modern, responsive styling with CSS Grid/Flexbox
- **AJAX** - Seamless asynchronous operations

### Authentication & Security
- **ASP.NET Core Identity** - Robust user management and authentication
- **Google OAuth 2.0** - Secure social login integration
- **Facebook OAuth** - Social authentication option
- **Role-based Authorization** - Granular access control system

### External Services Integration
- **Stripe** - PCI-compliant payment processing
- **Cloudinary** - Image storage, optimization, and delivery CDN
- **Gmail SMTP** - Reliable email notifications

## 📊 Database Schema

The application uses a comprehensive relational database schema:

###  ERD
![ERD](erd.png)

**Core Entities:**
- **ApplicationUser** - Extended Identity user with custom properties
- **Products** - Complete product catalog with images and metadata
- **Categories** - Hierarchical product categorization
- **Carts & CartItems** - Persistent shopping cart functionality
- **Orders & OrderItems** - Complete order management system
- **Reviews** - Product reviews with 5-star ratings
- **PromoCodes** - Flexible discount code system
- **WishlistItems** - Customer wishlist management
- **ProductNotifications** - Stock notification system

*Database relationships are optimized with proper indexing and foreign key constraints.*

## 🔒 Security Features

- **Multi-layer Authentication**: Local accounts + OAuth (Google/Facebook)
- **Role-based Authorization**: Granular permission system
- **Data Protection**: EF Core security with parameterized queries
- **Payment Security**: PCI-compliant Stripe integration
- **Input Validation**: Comprehensive model validation and sanitization
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Secure Headers**: Security headers for XSS protection

## 📈 Performance Optimizations

- **Smart Pagination**: Efficient data loading for large product catalogs
- **Image Optimization**: Cloudinary CDN with automatic image compression
- **Database Indexing**: Optimized queries with strategic indexing
- **Lazy Loading**: EF Core lazy loading for optimal performance
- **Async Operations**: Non-blocking database operations

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

### Getting Started
1. **Fork the repository**
2. **Create a feature branch**

## 🔮 Future Enhancements

- **Advanced Analytics**: Enhanced reporting dashboard with charts
- **Real-time Notifications**: WebSocket-based real-time updates
- **Advanced Search**: Elasticsearch integration for better search
- **Recommendation Engine**: AI-powered product recommendations

---

## 💡 Support

- ⭐ Star this repository if you find it helpful
- 🐛 [Report bugs](https://github.com/ahmedalian7/aurastore/issues/new?template=bug_report.md)
- 💡 [Request features](https://github.com/ahmedalian7/aurastore/issues/new?template=feature_request.md)

<div align="center">

**AURA Store** - Your complete e-commerce solution built with modern technologies and best practices.

**Built with ❤️ by the AuraStore team**

</div>
