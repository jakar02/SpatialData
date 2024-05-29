Nasz projekt składa się z kilku plików:
Class1.cs -najważniejsza, zawiera strukturę Point wraz z metodami do liczenia roznych odleglosci i pól
Program.cs - to nasze API
UnitTest1.cs - to testy metod to klasy Class1.cs w NUnit
SpatialData.sql - nasza baza danych, odczytuje dll

Jak uruchomić?
1. Utwórz nowy projekt (Class Library (.Net Framework)) i wkleić zawartość Class1.cs
2. Robimy project build i tworzymy w Debug DLL
3. W SSM utworzyć nową bazę danych o nazwie "SpatialDataProjectDB"
4. Wkleić zawartość SpatialData.sql oraz zmeinić lokalizację utworzonej DLL
5. Tworzymy zwykły projekt C# i wklejamy zawartość Program.cs
6. Zmieniamy connectionString w 11 lini na odpowiedni serwer
7. Tworzymy nowy projekt do Class1.cs do testów (NUnit) i wklejamy zawartość pliku UnitTest1.cs
8. Dodajemy zależność do projektu testującego z klasą Class1.cs
9. Zarówno API jak i testy powinny działać :)

