namespace E_Journal.Shared
{
    public class StudentStatus
    {
        private byte _score;

        public StudentStatus()
        {
            IsAttended = true;
        }

        public int Id { get; set; }
        public Student Student { get; set; }
        public bool IsAttended { get; set; }
        public byte Score
        {
            get => _score;
            set
            {
                if (IsAttended)
                {
                    throw new InvalidOperationException("Нельзя назначить оценку отсутствовавшему учащемуся.");
                }

                _score = value;
            }
        }
    }
}
