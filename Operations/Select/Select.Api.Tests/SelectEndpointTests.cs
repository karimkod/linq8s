using System.Net;
using System.Net.Mime;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace Select.Api.Tests;

public class SelectEndpointTests
{
    private readonly HttpClient _client;

    public SelectEndpointTests()
    {
        var webapp = new WebApplicationFactory<Program>();
        _client = webapp.CreateClient();
    }

    [Fact]
    public async Task GivenEmptyList_ThenReturnEmptyList()
    {
        using var body = new StringContent("""
                                           {
                                               "data": [],
                                               "fields": []
                                           }
                                           """, MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json));
        var result = await _client.PostAsync("/select", body);
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultBody = await result.Content.ReadAsStringAsync();
        resultBody.Should().Be("[]");
    }

    [Fact]
    public async Task GivenListWithSingleField_ThenReturnProjectedList()
    {
         using var body = new StringContent("""
                                            {
                                                "data": [{"name": "abdelkrim", "age":10}],
                                                "fields": ["name"]
                                            }
                                            """, MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json));
         var result = await _client.PostAsync("/select", body);
         result.StatusCode.Should().Be(HttpStatusCode.OK);
         var resultBody = await result.Content.ReadAsStringAsync();
         resultBody.Should().Be("""[{"name":"abdelkrim"}]""");       
    }
    
    [Fact]
    public async Task GivenListWithMultipleField_ThenReturnProjectedList()
    {
         using var body = new StringContent("""
                                            {
                                                "data": [{"name": "abdelkrim", "age":10, "birthplace": "Algeria"}],
                                                "fields": ["name", "age"]
                                            }
                                            """, MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json));
         var result = await _client.PostAsync("/select", body);
         result.StatusCode.Should().Be(HttpStatusCode.OK);
         var resultBody = await result.Content.ReadAsStringAsync();
         resultBody.Should().Be("""[{"name":"abdelkrim","age":10}]""");       
    }
    
    [Fact]
    public async Task GivenFieldThatDontExistInObject_ThenReturnNull()
    {
         using var body = new StringContent("""
                                            {
                                                "data": [{"name": "abdelkrim", "age":10, "birthplace": "Algeria"}],
                                                "fields": ["gender"]
                                            }
                                            """, MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json));
         var result = await _client.PostAsync("/select", body);
         result.StatusCode.Should().Be(HttpStatusCode.OK);
         var resultBody = await result.Content.ReadAsStringAsync();
         resultBody.Should().Be("""[{"gender":null}]""");       
    }
    
    
    
    [Fact(Skip = "Not supported yet.")]
    public async Task GivenListWithNestedJsonObject_ThenReturnProjectedList()
    {
         using var body = new StringContent("""
                                            {
                                                "data": [{"name": "abdelkrim", "age":10, "birthplace": "Algeria", "address": {"number": 1, "street": "Rue Daoudi"}}],
                                                "fields": ["name", "age", "address.number"]
                                            }
                                            """, MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json));
         var result = await _client.PostAsync("/select", body);
         result.StatusCode.Should().Be(HttpStatusCode.OK);
         var resultBody = await result.Content.ReadAsStringAsync();
         resultBody.Should().Be("""[{"name":"abdelkrim","age":10, "street.number": 1}]""");       
    }
}