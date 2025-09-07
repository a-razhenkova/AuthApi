## 🔐 How to Authenticate

### Client Authentication
- **POST** `/api/v1/tokens` → Obtain an access token using basic authorization.

### User Authentication
- **POST** `/api/v2/tokens` → Obtain access and refresh tokens.  
- **PUT** `/api/v2/tokens` → Refresh an access token.

### Multi-Factor Authentication (for users only)
- **POST** `/api/v1/mfa/otp` → Request an OTP.  
- **POST** `/api/v3/tokens` → Exchange OTP for access and refresh tokens.

### Token Validation
- **POST** `/api/v1/tokens/validate` → Validate an access token (works for both clients and users).
