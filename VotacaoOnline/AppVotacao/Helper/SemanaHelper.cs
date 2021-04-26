using System;
using System.Globalization;

public static class SemanaHelper
{

    public static DateTime PrimeiroDiaDaSemana(DateTime dayInWeek)
    {
        CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
        return PrimeiroDiaDaSemana(dayInWeek, defaultCultureInfo);
    }


    public static DateTime PrimeiroDiaDaSemana(DateTime diaNaSemana, CultureInfo cultureInfo)
    {
        DayOfWeek primeiroDia = cultureInfo.DateTimeFormat.FirstDayOfWeek;
        DateTime primeiroDiaNaSemana = diaNaSemana.Date;
        while (primeiroDiaNaSemana.DayOfWeek != primeiroDia)
            primeiroDiaNaSemana = primeiroDiaNaSemana.AddDays(-1);

        return primeiroDiaNaSemana;
    }
}