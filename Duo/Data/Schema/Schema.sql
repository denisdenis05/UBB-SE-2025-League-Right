-- Create Difficulty enum table
CREATE TABLE Difficulties (
    Id INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255)
);

-- Insert difficulty values
INSERT INTO Difficulties (Id, Name, Description) VALUES 
(1, 'Easy', 'Beginner level'),
(2, 'Medium', 'Intermediate level'),
(3, 'Hard', 'Advanced level');

-- Create Users table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(100) NOT NULL UNIQUE,
    LastCompletedSectionId INT NULL,
    LastCompletedQuizId INT NULL
);

-- Create Roadmap table
CREATE TABLE Roadmaps (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);

-- Create Sections table
CREATE TABLE Sections (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SubjectId INT NOT NULL,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    RoadmapId INT NOT NULL,
    OrderNumber INT NOT NULL,
    FOREIGN KEY (RoadmapId) REFERENCES Roadmaps(Id),
    CONSTRAINT UQ_Section_Roadmap_Order UNIQUE (RoadmapId, OrderNumber)
);

-- Create Exercises table (base table for exercise types)
CREATE TABLE Exercises (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Type VARCHAR(50) NOT NULL, -- 'MultipleChoice', 'FillInTheBlanks', 'Association'
    DifficultyId INT NOT NULL,
    FOREIGN KEY (DifficultyId) REFERENCES Difficulties(Id)
);

-- Create MultipleChoiceExercises table
CREATE TABLE MultipleChoiceExercises (
    ExerciseId INT PRIMARY KEY,
    Question VARCHAR(500) NOT NULL,
    CorrectAnswer VARCHAR(500) NOT NULL,
    FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id)
);

-- Create MultipleChoiceOptions table
CREATE TABLE MultipleChoiceOptions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ExerciseId INT NOT NULL,
    OptionText VARCHAR(500) NOT NULL,
    FOREIGN KEY (ExerciseId) REFERENCES MultipleChoiceExercises(ExerciseId)
);

-- Create FillInTheBlanksExercises table
CREATE TABLE FillInTheBlanksExercises (
    ExerciseId INT PRIMARY KEY,
    Sentence VARCHAR(500) NOT NULL,
    FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id)
);

-- Create FillInTheBlanksAnswers table
CREATE TABLE FillInTheBlanksAnswers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ExerciseId INT NOT NULL,
    CorrectAnswer VARCHAR(100) NOT NULL,
    Position INT NOT NULL,
    FOREIGN KEY (ExerciseId) REFERENCES FillInTheBlanksExercises(ExerciseId),
    CONSTRAINT UQ_FillInBlanks_Position UNIQUE (ExerciseId, Position)
);

-- Create AssociationExercises table
CREATE TABLE AssociationExercises (
    ExerciseId INT PRIMARY KEY,
    FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id)
);

-- Create AssociationPairs table
CREATE TABLE AssociationPairs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ExerciseId INT NOT NULL,
    FirstAnswer VARCHAR(255) NOT NULL,
    SecondAnswer VARCHAR(255) NOT NULL,
    FOREIGN KEY (ExerciseId) REFERENCES AssociationExercises(ExerciseId)
);

-- Create Quizzes table
CREATE TABLE Quizzes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SectionId INT NOT NULL,
    OrderNumber INT NOT NULL,
    FOREIGN KEY (SectionId) REFERENCES Sections(Id),
    CONSTRAINT UQ_Quiz_Section_Order UNIQUE (SectionId, OrderNumber)
);

-- Create QuizExercises table (junction table between Quizzes and Exercises)
CREATE TABLE QuizExercises (
    QuizId INT NOT NULL,
    ExerciseId INT NOT NULL,
    OrderNumber INT NOT NULL,
    PRIMARY KEY (QuizId, ExerciseId),
    FOREIGN KEY (QuizId) REFERENCES Quizzes(Id),
    FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id),
    CONSTRAINT UQ_QuizExercise_Order UNIQUE (QuizId, OrderNumber)
);

-- Create Exams table
CREATE TABLE Exams (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SectionId INT NOT NULL,
    FOREIGN KEY (SectionId) REFERENCES Sections(Id)
);

-- Create ExamExercises table (junction table between Exams and Exercises)
CREATE TABLE ExamExercises (
    ExamId INT NOT NULL,
    ExerciseId INT NOT NULL,
    OrderNumber INT NOT NULL,
    PRIMARY KEY (ExamId, ExerciseId),
    FOREIGN KEY (ExamId) REFERENCES Exams(Id),
    FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id),
    CONSTRAINT UQ_ExamExercise_Order UNIQUE (ExamId, OrderNumber)
);

-- Add foreign key constraints for Users table after all tables are created
ALTER TABLE Users
ADD CONSTRAINT FK_Users_LastCompletedSection 
    FOREIGN KEY (LastCompletedSectionId) REFERENCES Sections(Id);

ALTER TABLE Users
ADD CONSTRAINT FK_Users_LastCompletedQuiz 
    FOREIGN KEY (LastCompletedQuizId) REFERENCES Quizzes(Id);
