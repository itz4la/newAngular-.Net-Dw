# =============================================================================
# FILE    : conftest.py
# PURPOSE : Shared pytest fixtures – WebDriver setup/teardown and base URL.
#           Uses webdriver-manager to auto-download the correct ChromeDriver.
# NOTE    : AI Assistance Declaration – Selenium POM scaffold and fixture
#           patterns in this file were suggested by GitHub Copilot (Claude).
# =============================================================================

import pytest
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager

# ── Configuration ─────────────────────────────────────────────────────────────

BASE_URL  = "http://localhost:4200"   # Angular dev server
API_URL   = "http://localhost:5118"   # ASP.NET Core API


def pytest_addoption(parser):
    """Allow switching browser mode from the command line."""
    parser.addoption(
        "--headless",
        action="store_true",
        default=False,
        help="Run browser in headless mode (for CI).",
    )


def _build_chrome_options(headless=False):
    """Centralized Chrome options used by all fixtures."""
    options = Options()
    if headless:
        options.add_argument("--headless=new")
    options.add_argument("--no-sandbox")
    options.add_argument("--disable-dev-shm-usage")
    options.add_argument("--window-size=1920,1080")
    options.add_argument("--disable-gpu")
    return options


# ── WebDriver Fixture ─────────────────────────────────────────────────────────

@pytest.fixture(scope="session")
def driver(request):
    """
    Session-scoped Chrome WebDriver.
    Runs once for the entire test session (faster than per-function).
    """
    options = _build_chrome_options(headless=request.config.getoption("--headless"))

    service = Service(ChromeDriverManager().install())
    drv = webdriver.Chrome(service=service, options=options)
    drv.implicitly_wait(10)   # Global implicit wait: 10 seconds
    yield drv
    drv.quit()


@pytest.fixture(scope="function")
def fresh_driver(request):
    """
    Function-scoped Chrome WebDriver.
    Used by tests that require a clean browser state (e.g. login/logout tests).
    """
    options = _build_chrome_options(headless=request.config.getoption("--headless"))

    service = Service(ChromeDriverManager().install())
    drv = webdriver.Chrome(service=service, options=options)
    drv.implicitly_wait(10)
    yield drv
    drv.quit()
