using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        try
        {
            Shelf availableShelf = new Shelf();
            Shelf loanedShelf = new Shelf();

            AddBooksToShelf(availableShelf);

            PerformLibraryOperations(availableShelf, loanedShelf);

            PrintShelfContents("Books in Available Shelf:", availableShelf.GetAllBooks().Values);
            PrintShelfContents("Books by J.D. Salinger:", availableShelf.GetBooksByAuthor("J.D. Salinger"));
            PrintShelfContents("Books in Loaned Shelf:", loanedShelf.GetAllBooks().Values);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void AddBooksToShelf(Shelf shelf)
    {
        try
        {
            shelf.AddBook(new Book("The Great Gatsby", "F. Scott Fitzgerald", "Fiction", 4.5));
            shelf.AddBook(new Book("To Kill a Mockingbird", "Harper Lee", "Drama", 4.8));
            shelf.AddBook(new Book("1984", "George Orwell", "Dystopian", 4.2));
            shelf.AddBook(new Book("The Catcher in the Rye", "J.D. Salinger", "Fiction", 4.0));
            shelf.AddBook(new Book("Another Book", "J.D. Salinger", "Fiction", 4.2));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while adding books to the shelf: {ex.Message}");
        }
    }

    private static void PerformLibraryOperations(Shelf availableShelf, Shelf loanedShelf)
    {
        try
        {
            Console.WriteLine("\nLibrary Operations:");
            LibrarySystem.LoanBook(availableShelf.GetBookByTitle("The Great Gatsby"), availableShelf, loanedShelf);
            LibrarySystem.ReturnBook(availableShelf.GetBookByTitle("The Great Gatsby"), availableShelf, loanedShelf);
            bool isAvailable = LibrarySystem.IsBookAvailable(
                availableShelf.GetBookByTitle("The Great Gatsby"), availableShelf, loanedShelf);
            Console.WriteLine($"Is 'The Great Gatsby' available? {(isAvailable ? "Yes" : "No")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during library operations: {ex.Message}");
        }
    }

    private static void PrintShelfContents(string shelfName, IEnumerable<Book> books)
    {
        try
        {
            Console.WriteLine($"\n{shelfName}");
            foreach (Book book in books)
            {
                Console.WriteLine(LibrarySystem.FormatBookDetails(book));
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while printing shelf contents: {ex.Message}");
        }
    }
}

class Book
{
    private readonly string title;
    private readonly string author;
    private readonly string genre;
    private readonly double rating;

    public Book(string title, string author, string genre, double rating)
    {
        this.title = title ?? throw new ArgumentNullException(nameof(title), "Title cannot be null");
        this.author = author ?? throw new ArgumentNullException(nameof(author), "Author cannot be null");
        this.genre = genre ?? throw new ArgumentNullException(nameof(genre), "Genre cannot be null");
        this.rating = rating;
    }

    public string Title => title;
    public string Author => author;
    public string Genre => genre;
    public double Rating => rating;

    public override string ToString()
    {
        return $"Book{{title='{title}', author='{author}', genre='{genre}', rating={rating}}}";
    }
}

class Shelf
{
    private readonly Dictionary<string, Book> booksByTitle = new Dictionary<string, Book>();
    private readonly Dictionary<string, HashSet<Book>> booksByAuthor = new Dictionary<string, HashSet<Book>>();
    private readonly Dictionary<string, HashSet<Book>> booksByGenre = new Dictionary<string, HashSet<Book>>();

    public void AddBook(Book book)
    {
        if (book != null)
        {
            string title = book.Title;
            string author = book.Author;
            string genre = book.Genre;

            booksByTitle[title] = book;

            booksByAuthor.TryGetValue(author, out HashSet<Book> authorSet);
            authorSet ??= new HashSet<Book>();
            authorSet.Add(book);
            booksByAuthor[author] = authorSet;

            booksByGenre.TryGetValue(genre, out HashSet<Book> genreSet);
            genreSet ??= new HashSet<Book>();
            genreSet.Add(book);
            booksByGenre[genre] = genreSet;
        }
    }

    public Book GetBookByTitle(string title)
    {
        booksByTitle.TryGetValue(title, out Book book);
        return book;
    }

    public HashSet<Book> GetBooksByAuthor(string author)
    {
        booksByAuthor.TryGetValue(author, out HashSet<Book> authorSet);
        return authorSet ?? new HashSet<Book>();
    }

    public HashSet<Book> GetBooksByGenre(string genre)
    {
        booksByGenre.TryGetValue(genre, out HashSet<Book> genreSet);
        return genreSet ?? new HashSet<Book>();
    }

    public void RemoveBook(Book book)
    {
        if (book != null)
        {
            string title = book.Title;
            string author = book.Author;
            string genre = book.Genre;

            booksByTitle.Remove(title);

            if (booksByAuthor.TryGetValue(author, out HashSet<Book> authorSet))
            {
                authorSet.Remove(book);
                if (authorSet.Count == 0)
                {
                    booksByAuthor.Remove(author);
                }
            }

            if (booksByGenre.TryGetValue(genre, out HashSet<Book> genreSet))
            {
                genreSet.Remove(book);
                if (genreSet.Count == 0)
                {
                    booksByGenre.Remove(genre);
                }
            }
        }
    }

    public bool ContainsBook(Book book)
    {
        return book != null && booksByTitle.ContainsKey(book.Title);
    }

    public Dictionary<string, Book> GetAllBooks()
    {
        return new Dictionary<string, Book>(booksByTitle);
    }
}

class LibrarySystem
{
    public static string FormatBookDetails(Book book)
    {
        if (book != null)
        {
            return $"Title: {book.Title}\nAuthor: {book.Author}\nGenre: {book.Genre}\nRating: {book.Rating}";
        }

        return "Unknown Book";
    }

    public static bool LoanBook(Book book, Shelf availableShelf, Shelf loanedShelf)
    {
        try
        {
            if (book != null && availableShelf.ContainsBook(book))
            {
                availableShelf.RemoveBook(book);
                loanedShelf.AddBook(book);
                Console.WriteLine("Book loaned successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Book not available for loan.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while loaning the book: {ex.Message}");
            return false;
        }
    }

    public static bool ReturnBook(Book book, Shelf availableShelf, Shelf loanedShelf)
    {
        try
        {
            if (book != null && loanedShelf.ContainsBook(book))
            {
                loanedShelf.RemoveBook(book);
                availableShelf.AddBook(book);
                Console.WriteLine("Book returned successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid book return. Book not on loan.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while returning the book: {ex.Message}");
            return false;
        }
    }

    public static bool IsBookAvailable(Book book, Shelf availableShelf, Shelf loanedShelf)
    {
        try
        {
            return availableShelf.ContainsBook(book);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while checking book availability: {ex.Message}");
            return false;
        }
    }
}
