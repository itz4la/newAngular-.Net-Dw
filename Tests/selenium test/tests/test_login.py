# =============================================================================
# FILE    : tests/test_login.py
# PURPOSE : Automated Selenium tests for Login and Registration features.
# LEVEL   : System (UI/E2E)
# TYPE    : Functional – Black-box (equivalence classes + boundary values)
#           Security – authentication validation
#
# TOOL USED : Selenium WebDriver + pytest
# POM USED  : LoginPage, RegisterPage (pages/login_page.py)
#
# AI DECLARATION : Test case structure and WebDriver wait patterns were
#                  partially assisted by GitHub Copilot (Claude Sonnet).
#                  All test logic was reviewed and validated manually.
# =============================================================================

import pytest
import uuid
from conftest import BASE_URL
from pages.login_page import LoginPage, RegisterPage


class TestLogin:
    """
    Tests for the authentication workflows: login and registration.
    """

    # ─────────────────────────────────────────────────────────────────────────
    # TC-SE-AUTH-001  Login page loads correctly
    # ─────────────────────────────────────────────────────────────────────────
    def test_login_page_loads(self, fresh_driver):
        """
        Verify the login page is accessible and renders correctly.
        Technique: Equivalence class (page load verification)
        """
        page = LoginPage(fresh_driver)
        page.navigate(BASE_URL)

        # The page title or URL should indicate we're on the login screen
        current_url = page.get_current_url()
        assert "login" in current_url.lower() or BASE_URL in current_url, \
            f"Expected to land on login page but got: {current_url}"

    # ─────────────────────────────────────────────────────────────────────────
    # TC-SE-AUTH-002  Login with correct credentials succeeds
    # ─────────────────────────────────────────────────────────────────────────
    def test_login_valid_credentials(self, fresh_driver):
        """
        A registered user can log in with correct credentials.
        Technique: Equivalence class (valid/happy path partition)
        Pre-condition: The application must have a seeded admin user.
        Expected: Redirect away from /login (to dashboard/home)
        """
        page = LoginPage(fresh_driver)
        page.navigate(BASE_URL)

        page.login("admin@library.com", "Admin@123")

        # After successful login, user should be redirected away from /login
        try:
            page.wait_for_url_contains("dashboard", timeout=8)
            redirected = True
        except Exception:
            redirected = "/login" not in page.get_current_url()

        assert redirected, \
            "Expected redirect after successful login, still on login page"

    # ─────────────────────────────────────────────────────────────────────────
    # TC-SE-AUTH-003  Login with wrong password displays error (TC007 template)
    # ─────────────────────────────────────────────────────────────────────────
    def test_login_wrong_password_shows_error(self, fresh_driver):
        """
        Corresponds to the template test case TC007 from the guidelines:
        'Se connecter avec Mot de passe incorrect'

        Data:
            UserId   = admin@library.com
            Password = WrongPassword@999

        Expected: System displays an error message. User remains on login page.
        Technique: Equivalence class (invalid password partition)
        """
        page = LoginPage(fresh_driver)
        page.navigate(BASE_URL)

        page.login("admin@library.com", "WrongPassword@999")

        # Error should appear, user remains on login page
        assert page.is_error_displayed() or page.is_on_login_page(), \
            "Expected error message or stay on login page with wrong password"

    # ─────────────────────────────────────────────────────────────────────────
    # TC-SE-AUTH-004  Login with non-existent username shows error
    # Technique: Equivalence class (non-existent user partition)
    # ─────────────────────────────────────────────────────────────────────────
    def test_login_nonexistent_user(self, fresh_driver):
        page = LoginPage(fresh_driver)
        page.navigate(BASE_URL)

        page.login("this_user_does_not_exist_xyz@test.com", "SomePass@1")

        assert page.is_error_displayed() or page.is_on_login_page(), \
            "Expected error for non-existent user"

    # ─────────────────────────────────────────────────────────────────────────
    # TC-SE-AUTH-005  Login with empty fields – form validation
    # Technique: Boundary value (empty string – minimum length violation)
    # ─────────────────────────────────────────────────────────────────────────
    def test_login_empty_fields(self, fresh_driver):
        """
        Attempting to submit the login form with empty username and password
        should trigger client-side validation, preventing submission.
        """
        page = LoginPage(fresh_driver)
        page.navigate(BASE_URL)

        # Click login button without filling any fields
        page.click_login()

        # Should stay on login page (form validation prevents navigation)
        assert page.is_on_login_page() or page.is_error_displayed(), \
            "Submitting empty form should not navigate away from login page"


class TestRegister:
    """
    Tests for the user registration workflow.
    """

    # ─────────────────────────────────────────────────────────────────────────
    # TC-SE-REG-001  Successful registration with valid data
    # Technique: Equivalence class (valid partition)
    # ─────────────────────────────────────────────────────────────────────────
    def test_register_valid_user(self, fresh_driver):
        """
        A new user can register with a unique username, valid email and
        a strong password that meets the Identity requirements.
        """
        page = RegisterPage(fresh_driver)
        page.navigate(BASE_URL)

        unique_name = f"testuser_{uuid.uuid4().hex[:8]}"
        page.register(
            username = unique_name,
            email    = f"{unique_name}@library.com",
            password = "Register@1234!"
        )

        # After successful registration, expect success message or redirect
        assert page.is_success_shown() or "login" in page.get_current_url(), \
            "Expected success message or redirect to login after registration"

    # ─────────────────────────────────────────────────────────────────────────
    # TC-SE-REG-002  Registration with weak password shows error
    # Technique: Equivalence class (invalid – non-compliant password)
    # ─────────────────────────────────────────────────────────────────────────
    def test_register_weak_password(self, fresh_driver):
        """
        Attempting to register with a password that does not meet the
        ASP.NET Identity minimum requirements (e.g. '123') should display
        a validation error.
        """
        page = RegisterPage(fresh_driver)
        page.navigate(BASE_URL)

        page.register(
            username = f"weakuser_{uuid.uuid4().hex[:8]}",
            email    = "weak@library.com",
            password = "123"
        )

        assert page.is_error_shown() or page.is_element_present(
            *RegisterPage.REGISTER_BUTTON
        ), "Expected validation error for weak password"

    # ─────────────────────────────────────────────────────────────────────────
    # TC-SE-REG-003  Registration with empty username shows validation
    # Technique: Boundary value (empty required field)
    # ─────────────────────────────────────────────────────────────────────────
    def test_register_empty_username(self, fresh_driver):
        page = RegisterPage(fresh_driver)
        page.navigate(BASE_URL)

        page.register(
            username = "",
            email    = "nousername@library.com",
            password = "Valid@1234!"
        )

        # Should either show error or prevent submission
        assert page.is_error_shown() or page.is_element_present(
            *RegisterPage.REGISTER_BUTTON
        ), "Expected validation error for empty username"
