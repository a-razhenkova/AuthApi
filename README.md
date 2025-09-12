# :closed_lock_with_key: How to Authenticate

## :computer: Client Authentication
  
### Single-Factor Authentication
  
  An **access token** can be obtained using basic authentication with:\
  `POST /api/v1/tokens`
  
  > [!IMPORTANT]
  > External clients are required to activate a **subscription** via:\
  > `POST /api/v1/clients/{key}/subscriptions`
  
  > [!NOTE]
  > The **access token** can be validated via:\
  > `POST /api/v1/tokens/validation`.
  
---

## :iphone: User Authentication
  
### Single-Factor Authentication
  
  **Access and refresh tokens** can be obtained using:\
  `POST /api/v2/tokens`
  
### Multi-Factor Authentication
  
  1. An **OTP** can be requested via:\
    `POST /api/v1/mfa/otp`
  
  2. The **OTP** can be exchanged for **access and refresh tokens** via:\
     `POST /api/v3/tokens`
  
  > [!IMPORTANT]
  > To authenticate with MFA, users are required to verify their email via:\
  > `POST /api/v1/emails/verification/{token}`
  
  > [!NOTE]
  > The **access token** can be validated via:\
  > `POST /api/v1/tokens/validation`.
  
  > [!TIP]
  > The **access token** can be refreshed via:\
  > `PUT /api/v2/tokens`
  
---

:gear: Complete API documentation can be found in [`swagger.json`](./swagger.json).
