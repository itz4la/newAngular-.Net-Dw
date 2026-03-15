# =============================================================================
# FILE    : pages/base_page.py
# PURPOSE : Abstract base class for all Page Objects.
#           Encapsulates common Selenium helpers used across all pages.
# PATTERN : Page Object Model (POM) – recommended by the course guidelines.
# =============================================================================

from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support    import expected_conditions as EC
from selenium.webdriver.common.by  import By


class BasePage:
    """
    All page objects inherit from this class.
    Provides reusable wrappers around Selenium's most common operations,
    reducing code duplication and improving test maintainability.
    """

    # Default explicit wait timeout (seconds)
    TIMEOUT = 15

    def __init__(self, driver):
        self.driver = driver

    # ── Navigation ────────────────────────────────────────────────────────────

    def open(self, url: str):
        """Navigate the browser to the given URL."""
        self.driver.get(url)

    def get_current_url(self) -> str:
        return self.driver.current_url

    def get_page_title(self) -> str:
        return self.driver.title

    # ── Element interactions ──────────────────────────────────────────────────

    def find_element(self, by: By, locator: str):
        """Wait for element to be present, then return it."""
        return WebDriverWait(self.driver, self.TIMEOUT).until(
            EC.presence_of_element_located((by, locator))
        )

    def find_clickable(self, by: By, locator: str):
        """Wait for element to be clickable, then return it."""
        return WebDriverWait(self.driver, self.TIMEOUT).until(
            EC.element_to_be_clickable((by, locator))
        )

    def find_visible(self, by: By, locator: str):
        """Wait for element to be visible, then return it."""
        return WebDriverWait(self.driver, self.TIMEOUT).until(
            EC.visibility_of_element_located((by, locator))
        )

    def click(self, by: By, locator: str):
        """Find a clickable element and click it."""
        self.find_clickable(by, locator).click()

    def type_text(self, by: By, locator: str, text: str):
        """Clear any existing text and type the given text into the element."""
        element = self.find_clickable(by, locator)
        element.clear()
        element.send_keys(text)

    def get_text(self, by: By, locator: str) -> str:
        """Return the visible text content of an element."""
        return self.find_visible(by, locator).text

    def is_element_present(self, by: By, locator: str, timeout: int = 5) -> bool:
        """Return True if the element exists within the timeout, else False."""
        try:
            WebDriverWait(self.driver, timeout).until(
                EC.presence_of_element_located((by, locator))
            )
            return True
        except Exception:
            return False

    def wait_for_url_contains(self, fragment: str, timeout: int = None):
        """Wait until the current URL contains the given fragment."""
        t = timeout or self.TIMEOUT
        WebDriverWait(self.driver, t).until(EC.url_contains(fragment))

    def wait_for_text_in_element(self, by: By, locator: str, text: str):
        """Wait until the element's text contains the expected string."""
        WebDriverWait(self.driver, self.TIMEOUT).until(
            EC.text_to_be_present_in_element((by, locator), text)
        )

    def take_screenshot(self, filename: str):
        """Save a screenshot to the given filename (used on test failure)."""
        self.driver.save_screenshot(filename)
