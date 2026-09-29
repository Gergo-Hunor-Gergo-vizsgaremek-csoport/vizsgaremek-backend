using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Dtos;

public class LocationWriteDto
{
    [MaxLength(50)]
    public string Name { get; set; }
}