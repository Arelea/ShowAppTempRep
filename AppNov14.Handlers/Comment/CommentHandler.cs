using AppNov14.Handlers.Interfaces;
using AppNov14.Models.Base;
using AppNov14.Models.Comment;
using AppNov14.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Comment
{
    public sealed class CommentHandler : BaseDataHandler, ICommentHandler
    {
        private readonly ICommentRepository CommentRepository;

        private readonly IBaseDataRepository BaseDataRepository;

        public CommentHandler(ICommentRepository commentRepository, IBaseDataRepository baseDataRepository) : base(baseDataRepository)
        {
            CommentRepository = commentRepository;
            BaseDataRepository = baseDataRepository;
        }

        public List<CommentModel> GetList(int status)
        {
            return this.CommentRepository.GetList(status);
        }

        public MethodResult Create(CommentModel model)
        {
            return this.CommentRepository.Create(model);
        }

        public CommentModel GetComment(int id)
        {
            return this.CommentRepository.GetComment(id);
        }

        public MethodResult Close(int id, string answer)
        {
            return this.CommentRepository.Close(id, answer);
        }
    }
}