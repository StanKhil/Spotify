# 🎵 Spotify - Multi-Provider Audio Streaming Platform

A modern, full-featured **audio streaming platform** built with **.NET 10** that aggregates content from multiple music providers (local library, Spotify, and Jamendo). Stream music, podcasts, audiobooks, and manage your personal playlists with a robust backend architecture.

---

## 🚀 Quick Start

### Prerequisites

- **.NET 10 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **SQL Server** (LocalDB or full instance)
- **Visual Studio 2026** (Community Edition or higher) *(optional but recommended)*
- **Node.js** (for frontend, if applicable)

### Installation & Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/StanKhil/Spotify.git
   cd Spotify
   ```

2. **Install Dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure Database**
   - Update `appsettings.json` with your SQL Server connection string:
   ```json
   "ConnectionStrings": {
     "LocalDatabase": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Spotify;Integrated Security=True;"
   }
   ```

4. **Apply Database Migrations**
   ```bash
   cd Spotify.Web
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```
   The API will be available at: `https://localhost:7001`

---

## ✨ Key Features

### 🎧 Multi-Provider Content Aggregation
- **Local Library**: Upload and manage your personal music collection
- **Jamendo Integration**: Access millions of free, creative-commons licensed tracks
- **Spotify Integration**: Stream from your Spotify library *(OAuth 2.0 authentication)*
- **Unified Search & Discovery**: Search across all providers simultaneously

### 👤 User Management & Authentication
- **OAuth 2.0 with Google**: Seamless social login integration
- **JWT-based Authentication**: Secure token-based API access
- **Role-based Access Control (RBAC)**: Admin and user roles
- **User Profiles**: Customizable user settings and preferences
- **Email Notifications**: Authentication and notifications via Gmail SMTP

### 🎵 Content Management
- **Tracks**: Full metadata support (artist, album, duration, genre, mood)
- **Albums**: Album browsing with cover art and track listings
- **Podcasts & Episodes**: Podcast series management and episode playback
- **Audiobooks**: Complete audiobook library with chapters
- **Playlists**: Create and manage custom playlists
- **Genres, Moods & Tags**: Rich categorization system for content discovery

### ▶️ Playback & Audio Features
- **Local Audio Playback**: Stream from local file storage
- **Adaptive Bitrate Streaming**: Optimized audio quality
- **Playback History**: Track your listening habits
- **Like/Unlike System**: Mark favorite content
- **Recommendations**: Smart recommendations based on listening history
- **Configurable URL Expiration**: Time-limited audio URLs for security

### 🔍 Advanced Features
- **Smart Search**: Full-text search across all content types
- **Author Subscriptions**: Follow favorite artists and get updates
- **Dashboard Analytics**: User activity and platform statistics
- **Plugin System**: Extensible architecture for custom features
- **System Settings**: Configurable platform-wide settings

---

## 🏗️ Architecture & Technology Stack

### Backend Architecture: **Clean Architecture with Layered Design**

```
Spotify.Domain
├── Core business entities and logic
├── Enumerations (AudioProvider, Language)
└── Interfaces

Spotify.Application
├── DTOs (Data Transfer Objects)
├── Service interfaces
└── Business logic contracts

Spotify.Infrastructure
├── Database context and EF Core configurations
├── Service implementations
├── Authentication & Authorization
├── External API integrations (Jamendo, email)
├── File storage management
└── Playback services

Spotify.Web
├── ASP.NET Core REST API controllers
├── Endpoint definitions
├── Middleware & filters
└── Configuration
```

### Technology Stack

#### Core Framework
| Technology | Version | Purpose |
|-----------|---------|---------|
| **.NET** | 10.0 | Modern, performant runtime |
| **ASP.NET Core** | 10.0 | RESTful API framework |
| **Entity Framework Core** | 10.0 | Object-relational mapping (ORM) |

#### Database & Storage
| Technology | Purpose |
|-----------|---------|
| **SQL Server** | Primary relational database |
| **LocalDB** | Development database instance |
| **File System Storage** | Local audio file management |

#### Authentication & Security
| Technology | Purpose |
|-----------|---------|
| **JWT (JSON Web Tokens)** | API authentication |
| **OAuth 2.0** | Google social login |
| **ASP.NET Identity** | User management & roles |
| **HTTPS/TLS** | Transport security |

#### External Integrations
| Provider | Purpose |
|----------|---------|
| **Jamendo API** | Free music library access |
| **Google OAuth 2.0** | User authentication |
| **Gmail SMTP** | Email notifications |

#### NuGet Packages
```xml
<!-- Authentication -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="10.0.10" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.10" />

<!-- Database -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.10" />

<!-- API Documentation -->
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.10" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
```

---

## 📊 Database Schema Highlights

### Core Entities

**Content Types:**
- `AudioContent` (base class for all audio)
  - `Track` - Individual songs
  - `Episode` - Podcast episodes
  - `Audiobook` - Audiobook content

**Relationships:**
- `Author` ↔ `AuthorContent` ↔ `AudioContent` (One-to-Many)
- `Album` → `Tracks` (One-to-Many)
- `Podcast` → `Episodes` (One-to-Many)
- `User` → `Likes` (One-to-Many)
- `User` → `ListeningHistory` (One-to-Many)
- `User` → `Playlists` (One-to-Many)

**Categorization:**
- `Genre` - Music genres
- `Mood` - Audio moods/atmospheres
- `Tag` - Custom track tags
- `Language` - Content languages

---

## 🔐 Configuration Guide

### Essential Configurations (appsettings.json)

#### Database Connection
```json
"ConnectionStrings": {
  "LocalDatabase": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Spotify;Integrated Security=True;"
}
```

#### JWT Configuration
```json
"Jwt": {
  "Key": "VERY_LONG_KEY",
  "Issuer": "Spotify",
  "Audience": "SpotifyClient"
}
```

#### Email Service (Gmail SMTP)
```json
"Email": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "EnableSsl": true,
  "UserName": "your-email@gmail.com",
  "Password": "your-app-password",
  "FromAddress": "your-email@gmail.com",
  "FromName": "Spotify"
}
```

#### Google OAuth
```json
"Authentication": {
  "Google": {
    "ClientId": "your-google-client-id.apps.googleusercontent.com",
    "ClientSecret": "your-google-client-secret"
  }
},
"Google": {
  "RegistrationRedirectUrl": "http://localhost:3000/auth/google/complete",
  "LoginRedirectUrl": "http://localhost:3000/auth/google/success"
}
```

#### Jamendo API (Free Music Library)
```json
"Jamendo": {
  "ClientId": "ClientID",
  "BaseUrl": "https://api.jamendo.com/v3.0/",
  "AudioFormat": "mp32"
}
```

#### Audio Playback Settings
```json
"Playback": {
  "LocalStorageRoot": "App_Data/audio",
  "LocalUrlLifetimeMinutes": 10,
  "SigningKey": "8xLQx3n2vY6B5R7kPw1ZmA9cFs4HtU0NdXeJrMv82Qa"
}
```

---

## 🎯 Pros & Advantages

### ✅ **Multi-Provider Architecture**
- Stream from multiple sources without switching apps
- Unified search across local library, Jamendo, and Spotify
- Seamless provider switching

### ✅ **Modern Technology Stack**
- **.NET 10**: Latest, high-performance runtime
- **Clean Architecture**: Maintainable, testable codebase
- **Entity Framework Core**: Powerful ORM with LINQ support
- **RESTful API Design**: Industry-standard endpoint design

### ✅ **Enterprise-Grade Security**
- JWT token-based authentication
- OAuth 2.0 social login integration
- Role-based access control (RBAC)
- Token revocation system
- Encrypted sensitive data

### ✅ **Scalable & Extensible**
- Layered architecture allows easy feature addition
- Plugin system for extensibility
- Dependency injection for loose coupling
- Async/await for high-concurrency support

### ✅ **Rich Content Management**
- Support for multiple audio content types (tracks, episodes, audiobooks)
- Comprehensive metadata (genres, moods, tags, languages)
- Playlist creation and management
- Smart recommendation engine

### ✅ **User-Centric Features**
- Personalized listening history
- Like/unlike system for favorites
- Custom user preferences and settings
- Email notifications for important events
- Author subscription system

### ✅ **Developer-Friendly**
- Clear separation of concerns (Domain, Application, Infrastructure, Web)
- Well-documented API endpoints
- Easy database migrations with EF Core
- Comprehensive configuration options
- Logging and error handling

### ✅ **Free Content Access**
- Jamendo integration provides millions of free, legal tracks
- No licensing headaches for independent artists
- Support for creative-commons music

---

## 📁 Project Structure

```
Spotify/
├── Spotify.Domain/                 # Core domain entities & interfaces
│   ├── Entities/
│   │   ├── Content/               # Audio content types (Track, Album, etc.)
│   │   ├── User/                  # User entities (ApplicationUser, Profile)
│   │   ├── Location/              # Location entities (City, Country)
│   │   └── Security/              # Security configurations (JWT, Email)
│   └── Enumerations/              # Enum types (AudioProvider, Language)
│
├── Spotify.Application/            # Application logic & DTOs
│   ├── DTOs/                      # Data transfer objects
│   ├── Interfaces/                # Service interfaces
│   └── Services/                  # (if any)
│
├── Spotify.Infrastructure/         # Infrastructure & implementation
│   ├── Persistance/               # Database context & configurations
│   ├── Services/                  # Service implementations
│   ├── Authentication/            # Auth services
│   ├── Storage/                   # File storage
│   ├── Playback/                  # Audio playback handling
│   └── [Jamendo, Email, etc.]    # External integrations
│
└── Spotify.Web/                    # ASP.NET Core API
    ├── Controllers/               # REST endpoints
    ├── Middleware/                # Custom middleware
    ├── Program.cs                 # Dependency injection & configuration
    ├── appsettings.json          # Configuration file
    └── wwwroot/                   # Static files & audio storage
```

---

## 🔄 API Endpoints Overview

### Tracks
- `GET /api/tracks` - List all tracks
- `GET /api/tracks/{id}` - Get track details
- `POST /api/tracks` - Create new track
- `GET /api/track-actions/track-page/{trackId}` - Get track page with recommendations

### Albums
- `GET /api/albums` - List albums
- `GET /api/albums/{id}` - Get album details
- `POST /api/albums` - Create album

### Playlists
- `GET /api/playlists` - List user playlists
- `POST /api/playlists` - Create playlist
- `POST /api/playlists/{id}/tracks` - Add track to playlist

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login with credentials
- `GET /api/auth/google` - Google OAuth login

### Search
- `GET /api/search?query=...` - Search across all content

### User Profile
- `GET /api/user-profile` - Get current user profile
- `PUT /api/user-profile` - Update profile

---

## 🛠️ Development Workflow

### Building the Solution
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Creating Migrations
```bash
dotnet ef migrations add MigrationName --project Spotify.Infrastructure --startup-project Spotify.Web
```

### Applying Migrations
```bash
dotnet ef database update --project Spotify.Infrastructure --startup-project Spotify.Web
```

### Debug Mode
```bash
dotnet run --launch-profile https
```

---

## 📝 Environment Variables

For production, use environment variables instead of hardcoding secrets:

```bash
# Database
DB_CONNECTION_STRING=Server=...;Database=Spotify;...

# JWT
JWT_KEY=your-256-bit-base64-key
JWT_ISSUER=Spotify
JWT_AUDIENCE=SpotifyClient

# Google OAuth
GOOGLE_CLIENT_ID=...
GOOGLE_CLIENT_SECRET=...

# Email
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USERNAME=...
EMAIL_PASSWORD=...

# Jamendo
JAMENDO_CLIENT_ID=...
```

---

## 🚀 Deployment

### Publish Release Build
```bash
dotnet publish -c Release -o ./publish
```

### Docker Support (Optional)
Create a `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Spotify.Web.dll"]
```

---

## 📚 Additional Resources

- [.NET 10 Documentation](https://learn.microsoft.com/dotnet/)
- [ASP.NET Core API Docs](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Jamendo API](https://developer.jamendo.com/)

---

## 📄 License

This project is open-source and available under the MIT License.

---

## 👨‍💻 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

---

## 📧 Support

For questions or issues, please open a GitHub issue or contact the development team.

---

**Made with ❤️ using .NET 10**