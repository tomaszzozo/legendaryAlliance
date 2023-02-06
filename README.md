# Praca inżynierska

To repozytorium zawiera kod programu, który jest tematem mojej pracy inżynierskiej.
Tytuł roboczy gry to "Legendary Alliance".
Rzeczywisty tytuł pracy to "Strategiczna gra sieciowa – scenariusz oparty na zarządzaniu zasobami i przejmowaniu zasobów innych graczy".

## Uruchomienie gry

### Instalacja

#### MacOS

- Pobierz i wypakuj [archiwum](https://github.com/tomaszzozo/legendaryAlliance/releases/download/v1.0.2/MacOs.zip)
- Uruchom plik Build.app
- System MacOS może wyświetlić monit z pytaniem o potwierdzenie zamiaru uruchomienia aplikacji pochodzącej z internetu. Zatwierdź uruchomienie aplikacji.

#### Windows

- Pobierz i wypakuj [archiwum](https://github.com/tomaszzozo/legendaryAlliance/releases/download/v1.0.2/Windows.zip)
- Uruchom plik "My project.exe"
- Usługa Windows Defender może poinformować Cię, że ten plik jest niebezpieczny. Jest to spowodowane tylko i wyłącznie brakiem informacji o wydawcy, a nie złośliwym oprogramowaniem. Kliknij "więcej informacji", a pojawi się przycisk umożliwiający pominięcie tego monitu i uruchomienie programu.

## Rozpoczęcie i przebieg rozgrywki

### Logowanie lub rejestracja

Aby mieć możliwość rozgrywki, niezbędne jest zalogowanie się. Można do tego użyć gotowego konta tester1 lub tester2 z hasłem Tester123#, lub stworzyć nowe konto klikając w przycisk 'go to sign up' i wypełniając formularz.

### Gracz tworzący grę

- Kliknij w przycisk 'create game'
- Udostępnij wygenerowany kod rozgrywki (room id)
- Poczekaj na dołączenie drugiego gracza i ustawienie jego statusu gotowości do gry na 'ready'
- Kliknij w przycisk 'ready' aby ustawić swój status gotowości do gry, a następnie kliknij w przycisk 'start game'

### Gracz dołączający do gry

- Kliknij w przycisk 'join game'
- Pobierz od drugiego gracza tworzącego rozgrywkę kod gry (room id), a następnie wpisz go w pole tekstowe
- Kliknij w przycisk 'join room'
- Kliknij w przycisk 'ready' aby ustawić swój status gotowości do gry
- Czekaj na rozpoczęcie gry przez drugiego gracza

### Część wspólna

- Może wydawać się, że proces gry został zawieszony. Jest to jednak nieprawdą- wszystkie niezbędne obiekty ładują się w tle i należy odczekać maksymalnie 20 sekund.
- Jeżeli jesteś graczem tworzącym grę, kliknij na jakieś pole aby utworzyć tam swoją stolicę, a następnie naciśnij przycisk 'next turn'. Jeżeli nie jesteś tym graczem, czekaj na swoją turę i zrób to samo.
- Po wybraniu stolicy rozpoczyna się normalna rozgrywka. Klikając w swoje pole otworzysz jego widok, na którym wymienione są posiadane na tym polu obiekty i jednostki. Na tym ekranie masz możliwość ich kupna i sprzedaży. Szare przyciski lub brak obiektów oznacza, że nie spełniasz odpowiednich wymagań do kupna danego obiektu.
- Kliknięcie na inny teren wyświetli informację o właścicielu danego terenu oraz przybliżonej liczbie budynków i jednostek.
- Jeżeli wybrany teren sąsiaduje z twoim i nie jest jednocześnie twoim terenem, możesz kliknąć przycisk 'attack' aby przejść w tryb ataku
- Jeżeli wybrany teren sąsiaduje z twoim i jest jednocześnie twoim terenem, możesz kliknąć przycisk 'move' aby przegrupować swoje oddziały na ten teren
- W obu przypadkach należy wybrać wybraną ilość jednostek z sąsiadujących posiadanych terenów i kliknąć przycisk zatwierdzenia akcji.
- Uwaga: atak sąsiedniego pola wszystkimi jednostkami z danego pola pozbawia gracza kontroli nad danym polem!
- Wygrana następuje poprzez przejęcie wszystkich terenów przeciwnika, zdobycie 5000 punktów nauki lub 10000 sztuk złota.

## Uwagi

- Gra jest przeznaczona dla od dwóch do czterech graczy.
- Gracze muszą połączyć się na różnych maszynach, co spowodowane jest działaniem wybranego systemu rozwiązań sieciowych.
- Nie istnieje konieczność łączenia się na tych samych systemach operacyjnych.
- Użyte w grze obrazy są przechowywane na prywatnym dysku ze względu na ich duży rozmiar. Link do folderu assets: https://drive.google.com/drive/folders/1YdWnaP6O4MFLeEf-9QOhu3cQlUpxrUfZ?usp=sharing

## Wykorzystane technologie
- Języki programowania: C# oraz PHP
- Baza danych: MySQL
- Serwer: EC2 (Amazon Web Services)
- Silnik gry: Unity
- System kontroli wersji: Git oraz Github
- Rozwiązania sieciowe: Photon Unity Networking 2
