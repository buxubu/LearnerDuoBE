﻿using LearnerDuo.Helper;
using System.Text.Json;

namespace LearnerDuo.Extentions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage,
                                               int totalItems, int totalPages)
        {
            var paginationHeader = new PaginantionHeader(currentPage, itemsPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
