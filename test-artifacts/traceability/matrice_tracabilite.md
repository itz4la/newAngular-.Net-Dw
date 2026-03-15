# Matrice de Traçabilité des Tests

**Projet:** Système de Gestion de Bibliothèque Numérique
**Date:** 2026-03-15
**Réalisé par:** Ala Mesfar

## 1. Tests Unitaires .NET (XUnit)

| ID Test | Exigence | Scénario | Fichier Test | Statut |
|---------|----------|----------|--------------|--------|
| UT-DTO-001 | REQ-AUTH-01 | Stockage credentials LoginDTO | DtoTests.cs | PASS |
| UT-DTO-002 | REQ-AUTH-02 | Stockage données RegisterDTO | DtoTests.cs | PASS |
| UT-LOAN-001 | REQ-LOAN-01 | Récupération prêt par ID | LoanRepositoryTests.cs | PASS |
| UT-LOAN-002 | REQ-LOAN-02 | Création prêt valide | LoanRepositoryTests.cs | PASS |
| UT-LOAN-003 | REQ-LOAN-03 | Création prêt livre inexistant | LoanRepositoryTests.cs | PASS |
| UT-LOAN-004 | REQ-LOAN-04 | Retour de prêt | LoanRepositoryTests.cs | PASS |
| UT-LOAN-005 | REQ-LOAN-05 | Validation limite emprunts | LoanRepositoryTests.cs | PASS |
| UT-LOAN-006 | REQ-LOAN-06 | Mise à jour prêts en retard | LoanRepositoryTests.cs | PASS |
| UT-LOAN-007 | REQ-LOAN-07 | Filtrage et pagination prêts | LoanRepositoryTests.cs | PASS |

**Total Tests Unitaires:** 9 | **Réussis:** 9 | **Échoués:** 0

## 2. Tests Selenium Python (E2E)

| ID Test | Exigence | Scénario | Fichier Test | Statut |
|---------|----------|----------|--------------|--------|
| TC-SE-AUTH-001 | REQ-AUTH-01 | Chargement page login | test_login.py | NOT EXECUTED* |
| TC-SE-AUTH-002 | REQ-AUTH-02 | Login credentials valides | test_login.py | NOT EXECUTED* |
| TC-SE-AUTH-003 | REQ-AUTH-03 | Login mot de passe incorrect | test_login.py | NOT EXECUTED* |
| TC-SE-AUTH-004 | REQ-AUTH-04 | Login utilisateur inexistant | test_login.py | NOT EXECUTED* |
| TC-SE-AUTH-005 | REQ-AUTH-05 | Validation champs vides | test_login.py | NOT EXECUTED* |
| TC-SE-REG-001 | REQ-REG-01 | Inscription utilisateur valide | test_login.py | NOT EXECUTED* |
| TC-SE-REG-002 | REQ-REG-02 | Validation mot de passe faible | test_login.py | NOT EXECUTED* |
| TC-SE-REG-003 | REQ-REG-03 | Validation username vide | test_login.py | NOT EXECUTED* |
| TC-SE-BOOK-001 | REQ-BOOK-01 | Chargement page livres | test_books.py | NOT EXECUTED* |
| TC-SE-BOOK-002 | REQ-BOOK-02 | Affichage cartes livres | test_books.py | NOT EXECUTED* |
| TC-SE-BOOK-003 | REQ-BOOK-03 | Recherche filtrée livres | test_books.py | NOT EXECUTED* |
| TC-SE-BOOK-004 | REQ-BOOK-04 | Pagination livres | test_books.py | NOT EXECUTED* |
| TC-SE-BOOK-005 | REQ-BOOK-05 | Accès formulaire ajout livre | test_books.py | NOT EXECUTED* |
| TC-SE-BOOK-006 | REQ-BOOK-06 | Validation titre vide | test_books.py | NOT EXECUTED* |
| TC-SE-LOAN-001 | REQ-LOAN-UI-01 | Accessibilité page prêts | test_loans.py | NOT EXECUTED* |
| TC-SE-LOAN-002 | REQ-LOAN-UI-02 | Rendu tableau prêts | test_loans.py | NOT EXECUTED* |
| TC-SE-LOAN-003 | REQ-LOAN-UI-03 | Boutons retour prêts actifs | test_loans.py | NOT EXECUTED* |
| TC-SE-LOAN-004 | REQ-LOAN-UI-04 | Indicateur prêts en retard | test_loans.py | NOT EXECUTED* |
| TC-SE-LOAN-005 | REQ-LOAN-UI-05 | Affichage compteur prêts | test_loans.py | NOT EXECUTED* |
| TC-SE-LOAN-006 | REQ-LOAN-UI-06 | Workflow emprunt complet | test_loans.py | NOT EXECUTED* |

**Total Tests Selenium:** 20 | **Non exécutés:** 20

*\*Note: Les tests Selenium nécessitent que l'application soit en cours d'exécution:*
- *API .NET sur http://localhost:5118*
- *Frontend Angular sur http://localhost:4200*
- *Base de données configurée avec données de test*

## 3. Résumé de Couverture

| Niveau de Test | Type | Technique | Tests | Réussis | Échoués | Non Exécutés |
|----------------|------|-----------|-------|---------|---------|--------------|
| Unitaire | Fonctionnel | Boîte blanche | 9 | 9 | 0 | 0 |
| Système | Fonctionnel | Boîte noire | 20 | - | - | 20 |
| **Total** | | | **29** | **9** | **0** | **20** |

## 4. Exigences Non Couvertes

Les tests Selenium couvrent théoriquement toutes les exigences fonctionnelles principales mais n'ont pas pu être exécutés sans l'application en cours d'exécution.

## 5. Commandes d'Exécution

### Tests .NET
```bash
cd api/api.Tests
dotnet test --logger "console;verbosity=detailed"
```

### Tests Selenium
```bash
cd auto_test
python -m pytest tests/ --headless -v --html=../test-artifacts/reports/selenium_report.html
```
