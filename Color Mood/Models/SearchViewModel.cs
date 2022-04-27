using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Color_Mood.Models;

public class SearchViewModel
{
    public List<PaletteResultViewModel>? SearchResults { get; set; }
}