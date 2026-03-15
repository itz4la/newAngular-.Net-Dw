# =============================================================================
# FILE    : pages/login_page.py
# PURPOSE : Page Object for the Login and Register screens of the Angular app.
# =============================================================================

from selenium.webdriver.common.by import By
from .base_page import BasePage


class LoginPage(BasePage):
    """
    Represents the /login route of the Angular Library Management app.
    Encapsulates all locators and interactions for authentication.
    """

    # ── URL ───────────────────────────────────────────────────────────────────
    PATH = "/login"

    # ── Locators ──────────────────────────────────────────────────────────────
    # Using CSS selectors for robustness (no reliance on brittle XPath indices)
    USERNAME_INPUT    = (By.CSS_SELECTOR, "input[formcontrolname='username'], input[name='username'], input[placeholder*='sername'], input[placeholder*='Email']")
    PASSWORD_INPUT    = (By.CSS_SELECTOR, "input[type='password']")
    LOGIN_BUTTON      = (By.CSS_SELECTOR, "button[type='submit'], button.login-btn, button.btn-login")
    ERROR_MESSAGE     = (By.CSS_SELECTOR, ".error-message, .alert-danger, .invalid-feedback, [class*='error']")
    REGISTER_LINK     = (By.CSS_SELECTOR, "a[routerlink*='register'], a[href*='register']")

    # ── Actions ───────────────────────────────────────────────────────────────

    def navigate(self, base_url: str):
        """Open the login page."""
        self.open(base_url + self.PATH)

    def enter_username(self, username: str):
        self.type_text(*self.USERNAME_INPUT, username)

    def enter_password(self, password: str):
        self.type_text(*self.PASSWORD_INPUT, password)

    def click_login(self):
        self.click(*self.LOGIN_BUTTON)

    def login(self, username: str, password: str):
        """Convenience method: fill credentials and submit."""
        self.enter_username(username)
        self.enter_password(password)
        self.click_login()

    def get_error_message(self) -> str:
        """Return the text of the error/validation message shown on failure."""
        return self.get_text(*self.ERROR_MESSAGE)

    def is_error_displayed(self) -> bool:
        """Return True if an error message is visible on the page."""
        return self.is_element_present(*self.ERROR_MESSAGE)

    def is_on_login_page(self) -> bool:
        """Return True if the browser is currently showing the login page."""
        return "/login" in self.get_current_url()


class RegisterPage(BasePage):
    """
    Represents the /register route of the Angular Library Management app.
    """

    PATH = "/register"

    USERNAME_INPUT    = (By.CSS_SELECTOR, "input[formcontrolname='username'], input[name='username']")
    EMAIL_INPUT       = (By.CSS_SELECTOR, "input[type='email'], input[formcontrolname='email']")
    PASSWORD_INPUT    = (By.CSS_SELECTOR, "input[type='password'][formcontrolname='password'], input[name='password']")
    CONFIRM_PASS_INPUT= (By.CSS_SELECTOR, "input[type='password'][formcontrolname='confirmPassword'], input[name='confirmPassword']")
    REGISTER_BUTTON   = (By.CSS_SELECTOR, "button[type='submit'], button.register-btn")
    SUCCESS_MESSAGE   = (By.CSS_SELECTOR, ".success-message, .alert-success, [class*='success']")
    ERROR_MESSAGE     = (By.CSS_SELECTOR, ".error-message, .alert-danger, [class*='error']")

    def navigate(self, base_url: str):
        self.open(base_url + self.PATH)

    def register(self, username: str, email: str, password: str, confirm: str = None):
        """Fill and submit the registration form."""
        self.type_text(*self.USERNAME_INPUT, username)
        self.type_text(*self.EMAIL_INPUT, email)
        self.type_text(*self.PASSWORD_INPUT, password)
        if confirm is None:
            confirm = password
        if self.is_element_present(*self.CONFIRM_PASS_INPUT, timeout=2):
            self.type_text(*self.CONFIRM_PASS_INPUT, confirm)
        self.click(*self.REGISTER_BUTTON)

    def is_success_shown(self) -> bool:
        return self.is_element_present(*self.SUCCESS_MESSAGE)

    def is_error_shown(self) -> bool:
        return self.is_element_present(*self.ERROR_MESSAGE)
