# Instructions d'Exécution des Tests

**Projet:** Système de Gestion de Bibliothèque Numérique
**Réalisé par:** Ala Mesfar

---

## Prérequis

- .NET SDK 10.0+
- Node.js 18+ et npm
- Python 3.11+
- Google Chrome (pour Selenium)

---

## 1. Lancer les Tests Unitaires .NET

```bash
cd api/api.Tests
dotnet test --logger "console;verbosity=detailed"
```

**Résultat attendu:** 9 tests réussis (100%)

---

## 2. Lancer l'Application (requis pour Selenium)

### Terminal 1 - API .NET
```bash
cd api
dotnet run
```
Attendre: `Now listening on: http://localhost:5118`

### Terminal 2 - Frontend Angular
```bash
cd client
npm install    # première fois uniquement
npm start
```
Attendre: `Local: http://localhost:4200`

---

## 3. Lancer les Tests Selenium

### Terminal 3 - Tests Python
```bash
cd auto_test
pip install -r requirements.txt    # première fois uniquement
python -m pytest tests/ -v --html=../test-artifacts/reports/selenium_report.html
```

### Mode Headless (sans interface graphique)
```bash
python -m pytest tests/ --headless -v --html=../test-artifacts/reports/selenium_report.html
```

---

## 4. Données de Test

Les tests utilisent ces credentials:

| Utilisateur | Email | Mot de passe | Rôle |
|-------------|-------|--------------|------|
| Admin | admin@library.com | Admin@123 | Admin |
| Client | john.doe@library.com | Client@123 | Client |

**Important:** Ces utilisateurs doivent exister dans la base de données. Utilisez le endpoint `/api/seed` si nécessaire.

---

## 5. Structure des Tests

```
auto_test/
├── conftest.py          # Configuration WebDriver
├── pytest.ini           # Configuration Pytest
├── pages/               # Page Object Model
│   ├── base_page.py
│   ├── login_page.py
│   ├── books_page.py
│   └── loans_page.py
└── tests/
    ├── test_login.py    # 8 tests authentification
    ├── test_books.py    # 6 tests livres
    └── test_loans.py    # 6 tests prêts
```

---

## 6. Rapports Générés

Après exécution:
- `test-artifacts/reports/selenium_report.html` - Rapport HTML des tests Selenium
- `test-artifacts/reports/rapport_execution_tests.md` - Rapport d'exécution
- `Rapport_Test_Qualite_Logiciel_Ala_Mesfar.docx` - Rapport final complet

---

## 7. Résolution de Problèmes

### ERR_CONNECTION_REFUSED
L'application n'est pas démarrée. Lancez l'API et Angular d'abord.

### ChromeDriver introuvable
```bash
pip install webdriver-manager
```
Le driver sera téléchargé automatiquement.

### Tests qui timeout
Augmentez le timeout dans `conftest.py` ou vérifiez la connexion à la base de données.

---

## Contact

Pour toute question: Ala Mesfar
