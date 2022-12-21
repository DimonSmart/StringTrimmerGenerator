namespace DimonSmart.StringTrimmerGeneratorDemo
{
    public static class UseStringTrimmerGenerator
    {
        public static void Run()
        {
            var model = new StringTrimmerExampleModel();
            // All generated methods
            model.TrimExtraSpaces();
            model.TrimAll();
            model.TrimStart();
            model.TrimEnd();

            // This method is recommended in most cases
            model.TrimAll();
        }
    }
}
