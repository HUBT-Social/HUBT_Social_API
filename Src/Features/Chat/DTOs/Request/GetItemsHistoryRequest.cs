
using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Chat.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Utilities.IO;

public class GetItemsHistoryRequest : GetHistoryRequest
{
    public List<string>? Types { get; set; } = null;
}
