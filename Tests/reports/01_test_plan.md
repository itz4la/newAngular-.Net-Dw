% =============================================================================
% DOCUMENT : 01_test_plan.md
% TITLE    : Plan de Test – Système de Gestion de Bibliothèque
% MODULE   : Test et Qualité Logiciel
% FORMAT   : LaTeX / Overleaf
%
% INSTRUCTIONS : Copiez ce contenu dans un nouveau projet Overleaf.
%                Compilez avec pdfLaTeX.
% =============================================================================

```latex
\documentclass[12pt,a4paper]{report}

% ── Packages ──────────────────────────────────────────────────────────────────
\usepackage[utf8]{inputenc}
\usepackage[T1]{fontenc}
\usepackage[french]{babel}
\usepackage{geometry}
\usepackage{graphicx}
\usepackage{booktabs}
\usepackage{longtable}
\usepackage{array}
\usepackage{xcolor}
\usepackage{hyperref}
\usepackage{fancyhdr}
\usepackage{titlesec}
\usepackage{enumitem}
\usepackage{multirow}

\geometry{margin=2.5cm}

% ── Colors ────────────────────────────────────────────────────────────────────
\definecolor{primary}{RGB}{0,84,166}
\definecolor{secondary}{RGB}{0,150,136}
\definecolor{lightgray}{RGB}{245,245,245}

% ── Header / Footer ───────────────────────────────────────────────────────────
\pagestyle{fancy}
\fancyhf{}
\rhead{\textcolor{primary}{\textbf{Plan de Test}}}
\lhead{Système de Gestion de Bibliothèque}
\cfoot{\thepage}
\renewcommand{\headrulewidth}{0.4pt}

% ── Section formatting ────────────────────────────────────────────────────────
\titleformat{\chapter}[block]{\Large\bfseries\color{primary}}{}{0em}{}[\titlerule]
\titleformat{\section}{\large\bfseries\color{secondary}}{}{0em}{}
\titleformat{\subsection}{\normalsize\bfseries}{}{0em}{}

% ── Document ──────────────────────────────────────────────────────────────────
\begin{document}

% ── Title Page ────────────────────────────────────────────────────────────────
\begin{titlepage}
  \centering
  \vspace*{3cm}
  {\LARGE\bfseries\color{primary} Plan de Test\\[0.5em]
   Système de Gestion de Bibliothèque\par}
  \vspace{1.5cm}
  \rule{\linewidth}{0.5pt}
  \vspace{0.5cm}
  {\large Module : Test et Qualité Logiciel\par}
  \vspace{0.5cm}
  \rule{\linewidth}{0.5pt}
  \vspace{2cm}
  \begin{tabular}{ll}
    \textbf{Projet :}    & Gestion de Bibliothèque (.NET + Angular) \\[6pt]
    \textbf{Version :}   & 1.0 \\[6pt]
    \textbf{Date :}      & \today \\[6pt]
    \textbf{Statut :}    & Final \\
  \end{tabular}
  \vfill
  {\large Année académique 2025--2026\par}
\end{titlepage}

\tableofcontents
\newpage

% =============================================================================
\chapter{Introduction}
% =============================================================================

\section{Objectif du document}
Ce document constitue le \textbf{Plan de Test} officiel du projet de gestion
de bibliothèque développé dans le cadre du module \textit{Framework .NET}.
Il définit la stratégie, la portée, les ressources et le calendrier de
l'ensemble des activités de test.

\section{Système Sous Test (SUT)}
Le Système Sous Test est une application web complète comprenant :
\begin{itemize}[noitemsep]
  \item \textbf{Backend :} API REST développée avec ASP.NET Core (.NET 10),
        Entity Framework Core, Identity et JWT.
  \item \textbf{Frontend :} Application SPA développée avec Angular 18+.
  \item \textbf{Base de données :} SQL Server (production) /
        In-Memory EF Core (tests).
\end{itemize}

\section{Fonctionnalités couvertes}
\begin{itemize}[noitemsep]
  \item Authentification (inscription, connexion JWT).
  \item Gestion des livres (CRUD complet).
  \item Gestion des genres (CRUD complet).
  \item Gestion des emprunts (création, retour, vérification disponibilité).
  \item Tableau de bord analytique (Data Warehouse).
  \item Gestion des utilisateurs (Admin CRUD).
\end{itemize}

% =============================================================================
\chapter{Portée des Tests}
% =============================================================================

\section{Périmètre inclus}
\begin{longtable}{>{\bfseries}p{3cm} p{11cm}}
\toprule
Composant & Description des tests \\
\midrule
\endhead
Backend API & Tests unitaires des contrôleurs, tests d'intégration HTTP,
             tests de validation des DTOs et des règles métier. \\[4pt]
Authentification & Tests de connexion (succès, mot de passe incorrect,
                  utilisateur inexistant), tests d'inscription. \\[4pt]
Gestion livres & CRUD complet : création, lecture, mise à jour, suppression,
                 filtrage et pagination. \\[4pt]
Gestion emprunts & Création d'emprunt, retour, vérification limites (max 5),
                   disponibilité, emprunts actifs. \\[4pt]
Frontend Angular & Tests UI avec Selenium WebDriver via le modèle POM. \\[4pt]
\bottomrule
\end{longtable}

\section{Périmètre exclu}
\begin{itemize}[noitemsep]
  \item Tests de charge et montée en charge (hors scope du module).
  \item Tests de la couche Data Warehouse (AnalyticsController) en profondeur.
  \item Tests de compatibilité IE11 (navigateur obsolète).
\end{itemize}

% =============================================================================
\chapter{Stratégie de Test}
% =============================================================================

\section{Niveaux de test}

\begin{longtable}{>{\bfseries}p{3.5cm} p{3.5cm} p{7cm}}
\toprule
Niveau & Outils & Description \\
\midrule
\endhead
Tests unitaires & xUnit + Moq + FluentAssertions
  & Isolation des contrôleurs via mocks des repositories.
    Vérification du comportement de chaque méthode. \\[4pt]
Tests d'intégration & xUnit + WebApplicationFactory + EF InMemory
  & Tests HTTP end-to-end avec base de données in-memory.
    Vérification des interactions entre composants. \\[4pt]
Tests système & xUnit + WebApplicationFactory
  & Scénarios utilisateurs complets : inscription → emprunt → retour. \\[4pt]
Tests UI (système) & Selenium + pytest + POM
  & Automatisation des parcours utilisateurs via navigateur Chrome. \\
\bottomrule
\end{longtable}

\section{Types de tests}

\subsection{Tests fonctionnels}
\begin{itemize}[noitemsep]
  \item \textbf{Tests basés sur les exigences :} chaque endpoint API est
        testé conformément à sa spécification.
  \item \textbf{Tests de confirmation :} vérification que les corrections
        de bugs n'ont pas réintroduit de régressions.
  \item \textbf{Tests de régression :} couverture des scénarios critiques
        après chaque modification.
\end{itemize}

\subsection{Tests non fonctionnels}
\begin{itemize}[noitemsep]
  \item \textbf{Sécurité basique :} tests des limites d'authentification
        (token JWT, accès non autorisé, injection SQL via paramètres).
  \item \textbf{Compatibilité navigateur :} tests Selenium sur Chrome en
        mode headless.
  \item \textbf{Ergonomie :} vérification de la présence des messages
        d'erreur et des indicateurs visuels.
\end{itemize}

\section{Techniques de test utilisées}

\begin{longtable}{>{\bfseries}p{4cm} p{10cm}}
\toprule
Technique & Application \\
\midrule
\endhead
Partitionnement en classes d'équivalence
  & Séparation des entrées valides / invalides pour chaque paramètre.
    Ex. : ID $> 0$ (valide) vs ID $\leq 0$ (invalide). \\[4pt]
Analyse des valeurs limites
  & Tests aux bornes : ID = 0, ID = -1, limite de 5 emprunts. \\[4pt]
Tests boîte noire
  & Majority des tests unitaires et d'intégration : focus sur les
    entrées/sorties sans connaissance de l'implémentation. \\[4pt]
Tests boîte blanche
  & Analyse statique du code source, revue de code entre membres. \\
\bottomrule
\end{longtable}

% =============================================================================
\chapter{Environnement de Test}
% =============================================================================

\section{Configuration matérielle et logicielle}

\begin{longtable}{p{4cm} p{10cm}}
\toprule
\textbf{Composant} & \textbf{Configuration} \\
\midrule
\endhead
Système d'exploitation & Windows 11 / Ubuntu 22.04 \\[4pt]
Runtime .NET & .NET 10.0 SDK \\[4pt]
Base de données (tests) & EF Core InMemory Provider \\[4pt]
Base de données (prod) & SQL Server 2022 \\[4pt]
Navigateur & Google Chrome (dernier) – headless \\[4pt]
Python & Python 3.11+ \\[4pt]
IDE & Visual Studio 2022 / VS Code \\[4pt]
CI/CD & Exécution locale (dotnet test + pytest) \\
\bottomrule
\end{longtable}

\section{Données de test}
Les données de test sont générées via :
\begin{itemize}[noitemsep]
  \item \textbf{SeedDataController} : 20 livres, 4 genres, 3 utilisateurs,
        plusieurs emprunts prédéfinis.
  \item \textbf{Génération UUID} : noms d'utilisateurs et titres uniques
        pour l'isolation des tests d'intégration.
  \item \textbf{Fixtures pytest} : données injectées avant chaque test Selenium.
\end{itemize}

% =============================================================================
\chapter{Planification}
% =============================================================================

\section{Critères d'entrée}
\begin{itemize}[noitemsep]
  \item Le code source du backend et du frontend est disponible et compilable.
  \item L'environnement de test est configuré (.NET SDK, Python, Chrome).
  \item Les exigences fonctionnelles sont documentées.
\end{itemize}

\section{Critères de sortie}
\begin{itemize}[noitemsep]
  \item Tous les tests critiques (P1) passent avec succès.
  \item Aucun défaut bloquant non résolu.
  \item Couverture minimale : 80\% des cas de test prévus exécutés.
  \item Rapport final de test complété et validé.
\end{itemize}

\section{Critères de suspension}
Les tests sont suspendus si :
\begin{itemize}[noitemsep]
  \item L'environnement de build est non fonctionnel.
  \item Plus de 30\% des tests bloquants échouent simultanément.
\end{itemize}

% =============================================================================
\chapter{Ressources et Responsabilités}
% =============================================================================

\begin{longtable}{p{3.5cm} p{5cm} p{5.5cm}}
\toprule
\textbf{Rôle} & \textbf{Responsabilités} & \textbf{Outils} \\
\midrule
\endhead
Test Lead & Planification, revue du plan, rapport final
  & GitHub, LaTeX \\[4pt]
Testeur Backend & Tests unitaires et d'intégration .NET
  & xUnit, Moq, FluentAssertions \\[4pt]
Testeur Frontend & Tests Selenium, POM, scénarios UI
  & Python, Selenium, pytest \\[4pt]
Testeur Statique & Revue de code, analyse statique
  & Visual Studio Analyzer, SonarLint \\
\bottomrule
\end{longtable}

% =============================================================================
\chapter{Outils de Test}
% =============================================================================

\begin{longtable}{p{4cm} p{4cm} p{6cm}}
\toprule
\textbf{Outil} & \textbf{Catégorie} & \textbf{Usage} \\
\midrule
\endhead
xUnit & Framework de test & Tests unitaires et d'intégration .NET \\[4pt]
Moq & Mocking & Simulation des repositories dans les tests unitaires \\[4pt]
FluentAssertions & Assertions & Assertions lisibles et expressives \\[4pt]
WebApplicationFactory & Intégration & Boot de l'API dans les tests HTTP \\[4pt]
EF Core InMemory & Base de données & Substitut in-memory pour les tests \\[4pt]
Selenium WebDriver & Automatisation UI & Tests E2E via navigateur Chrome \\[4pt]
pytest & Framework Python & Exécution et rapport des tests Selenium \\[4pt]
webdriver-manager & Gestion drivers & Téléchargement automatique ChromeDriver \\[4pt]
pytest-html & Rapport & Génération du rapport HTML des tests Selenium \\[4pt]
GitHub Copilot & IA (déclaré) & Génération de scaffolding de tests, suggestions \\
\bottomrule
\end{longtable}

\section{Déclaration d'utilisation d'IA}
\begin{itemize}[noitemsep]
  \item \textbf{Outil utilisé :} GitHub Copilot (Claude Sonnet).
  \item \textbf{Usage :} Génération du scaffold des Page Objects, suggestion
        des patterns xUnit/Moq, génération du code LaTeX.
  \item \textbf{Validation :} Toute suggestion IA a été relue, corrigée et
        validée manuellement par l'équipe de test.
\end{itemize}

% =============================================================================
\chapter{Risques et Mitigation}
% =============================================================================

\begin{longtable}{p{5cm} p{2cm} p{7cm}}
\toprule
\textbf{Risque} & \textbf{Priorité} & \textbf{Mitigation} \\
\midrule
\endhead
Tests Selenium fragiles si l'UI change & Haute
  & Utilisation du POM pour isoler les locators. \\[4pt]
Données de test non reproductibles & Haute
  & UUID uniques + reset de la BDD in-memory par test. \\[4pt]
Compatibilité .NET 10 (version récente) & Moyenne
  & Fixation des versions de packages dans le .csproj. \\[4pt]
Tests d'intégration lents & Faible
  & Scope \texttt{IClassFixture} pour réutiliser la factory. \\
\bottomrule
\end{longtable}

\end{document}
```
