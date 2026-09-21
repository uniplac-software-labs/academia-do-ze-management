using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.Mappings;

public static class MatriculaEnumMappingExtensions
{
    public static MatriculaPlano ToDomain(this AppMatriculaPlano appPlano) => (MatriculaPlano)appPlano;
    public static AppMatriculaPlano ToApplication(this MatriculaPlano domainPlano) => (AppMatriculaPlano)domainPlano;

    public static MatriculaRestricoes ToDomain(this AppMatriculaRestricoes appRestricoes) => (MatriculaRestricoes)appRestricoes;
    public static AppMatriculaRestricoes ToApplication(this MatriculaRestricoes domainRestricoes) => (AppMatriculaRestricoes)domainRestricoes;
}
