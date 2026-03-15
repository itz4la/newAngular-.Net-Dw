# Rapport d'Exécution des Tests

**Date d'exécution:** 2026-03-15
**Environnement:** Windows 10 (10.0.26200)
**Réalisé par:** Ala Mesfar

## 1. Résumé Exécutif

| Métrique | Valeur |
|----------|--------|
| Tests .NET exécutés | 9 |
| Tests .NET réussis | 9 |
| Tests .NET échoués | 0 |
| Taux de réussite .NET | 100% |
| Durée d'exécution .NET | 2.91 secondes |
| Tests Selenium prévus | 20 |
| Tests Selenium exécutés | 0* |

*Tests Selenium non exécutés car l'application n'était pas en cours d'exécution.

## 2. Environnement de Test

### Outils et Versions
- **.NET SDK:** 10.0.100
- **Python:** 3.11.1
- **XUnit:** 2.9.2
- **Selenium:** 4.41.0
- **pytest:** 9.0.2
- **Chrome WebDriver:** Auto-managed via webdriver-manager

### Configuration Matérielle
- **OS:** Windows 10 (Build 26200)
- **Platform:** win32 (x64)

## 3. Résultats Détaillés des Tests .NET

### 3.1 Tests DTO (api.Tests.Domaine.DtoTests)

| Test | Durée | Statut |
|------|-------|--------|
| RegisterDto_StoresUserData | 164 ms | PASS |
| LoginDto_StoresCredentials | < 1 ms | PASS |

### 3.2 Tests Repository (api.Tests.Repositories.LoanRepositoryTests)

| Test | Durée | Statut |
|------|-------|--------|
| CreateLoanAsync_ReturnsNull_WhenBookDoesNotExist | 828 ms | PASS |
| GetByIdAsync_ReturnsMappedLoan_WhenLoanExists | 137 ms | PASS |
| CreateLoanAsync_CreatesLoan_WhenValidationPasses | 28 ms | PASS |
| UpdateOverdueLoansAsync_MarksActivePastDueLoansAsOverdue | 11 ms | PASS |
| ValidateLoanCreationAsync_Fails_WhenUserReachedMaxBooks | 12 ms | PASS |
| GetAllAsync_AppliesStatusFilterAndPaging | 28 ms | PASS |
| ReturnLoanAsync_SetsReturnedStatus_AndReturnDate | 3 ms | PASS |

### 3.3 Analyse des Temps d'Exécution

- **Test le plus rapide:** LoginDto_StoresCredentials (< 1 ms)
- **Test le plus long:** CreateLoanAsync_ReturnsNull_WhenBookDoesNotExist (828 ms)
- **Temps moyen:** ~130 ms

Le test le plus long correspond à l'initialisation de la base de données in-memory, ce qui est normal pour le premier test de la série.

## 4. Résultats des Tests Selenium

### 4.1 Tentative d'Exécution

Une tentative d'exécution a été effectuée avec la commande:
```bash
python -m pytest tests/test_login.py::TestLogin::test_login_page_loads --headless -v
```

### 4.2 Résultat
**Statut:** ÉCHEC - net::ERR_CONNECTION_REFUSED

**Cause:** L'application Angular n'était pas en cours d'exécution sur http://localhost:4200

### 4.3 Prérequis pour l'Exécution

Pour exécuter les tests Selenium avec succès:

1. **Démarrer l'API .NET:**
   ```bash
   cd api
   dotnet run
   ```

2. **Démarrer le frontend Angular:**
   ```bash
   cd client
   npm start
   ```

3. **S'assurer que la base de données est configurée** avec les données de test (admin@library.com / Admin@123)

4. **Exécuter les tests Selenium:**
   ```bash
   cd auto_test
   python -m pytest tests/ --headless -v --html=../test-artifacts/reports/selenium_report.html
   ```

## 5. Anomalies Détectées

### 5.1 Pendant la Compilation
- **67 avertissements** liés aux types nullables (CS8618, CS8603, etc.)
- **Aucune erreur** de compilation bloquante

### 5.2 Pendant l'Exécution
- **Aucun bug fonctionnel** détecté dans les tests unitaires
- **Tous les tests passent** avec succès

## 6. Recommandations

1. **Intégration Continue:** Configurer GitHub Actions pour exécuter automatiquement les tests .NET à chaque commit
2. **Tests Selenium:** Intégrer dans un pipeline CI avec containers Docker pour garantir l'environnement
3. **Nullable Types:** Résoudre progressivement les avertissements CS8618 pour améliorer la robustesse

## 7. Conclusion

Les tests unitaires .NET démontrent un niveau de qualité satisfaisant avec un taux de réussite de 100%. Les tests Selenium sont prêts à être exécutés mais nécessitent l'application en cours d'exécution, ce qui est documenté de manière transparente dans ce rapport.
