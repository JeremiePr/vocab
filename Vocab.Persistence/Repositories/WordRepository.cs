﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocab.Application.Contracts.Persistence;
using Vocab.Domain.Entities;

namespace Vocab.Persistence.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly VocabContext _context;

        public WordRepository(VocabContext context)
        {
            _context = context;
        }

        public Task<List<Word>> GetAll()
        {
            return _context.Words
                .ToListAsync();
        }

        public Task<Word> GetOneById(int id)
        {
            return _context.Words
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Word> GetOneRandomly(List<int> categoryIds)
        {
            var random = new Random();
            return await _context.WordCategories
                .Include(x => x.Word)
                .Where(x => !categoryIds.Any() || categoryIds.Contains(x.CategoryId))
                .GroupBy(x => x.Word)
                .Select(x => x.Key)
                .OrderBy(x => random.Next())
                .Take(1)
                .FirstOrDefaultAsync();
        }

        public Task<List<Word>> Get(List<int> categoryIds, string inputKeyWord, string inputValueWord)
        {
            return _context.WordCategories
                .Include(x => x.Word)
                .Where(x => !categoryIds.Any() || categoryIds.Contains(x.CategoryId))
                .Where(x => x.Word.KeyWord.StartsWith(inputKeyWord))
                .Where(x => x.Word.ValueWord.Contains(inputValueWord))
                .GroupBy(x => x.Word)
                .Select(x => x.Key)
                .ToListAsync();
        }

        public async Task<Word> Create(Word word)
        {
            _context.Words.Add(word);
            await _context.SaveChangesAsync();
            return word;
        }

        public async Task<Word> Update(Word word)
        {
            _context.Words.Attach(word);
            _context.Entry(word).Property(x => x.KeyWord).IsModified = true;
            _context.Entry(word).Property(x => x.ValueWord).IsModified = true;
            await _context.SaveChangesAsync();
            return word;
        }

        public async Task UpdateCategories(int id, List<int> categoryIds)
        {
            foreach (var relation in _context.WordCategories.Where(x => x.WordId == id))
            {
                _context.WordCategories.Remove(relation);
            }
            foreach (var categoryId in categoryIds)
            {
                _context.WordCategories.Add(new WordCategory { WordId = id, CategoryId = categoryId });
            }
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int wordId)
        {
            foreach (var relation in _context.WordCategories.Where(x => x.WordId == wordId))
            {
                _context.WordCategories.Remove(relation);
            }
            var word = new Word { Id = wordId, IsActive = false };
            _context.Words.Attach(word);
            _context.Entry(word).Property(x => x.IsActive).IsModified = true;
            await _context.SaveChangesAsync();
        }
    }
}
