using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Models;

    

    public class TextRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class ModerationResult
    {
        public double RiskScore { get; set; }  // 0 تا 100
        public bool IsSafe { get; set; }
        public List<string> Flags { get; set; } = new();
    }

