using AppNov14.Helpers;
using AppNov14.Models.Base;
using AppNov14.Models.Comment;
using AppNov14.Repositories.Extensions;
using AppNov14.Repositories.Interfaces;
using AppNov14.SqlDataAccess;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace AppNov14.Repositories
{
    public sealed class CommentRepository : BaseDataRepository, ICommentRepository
    {
        public CommentRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public List<CommentModel> GetList(int status)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var query = context.Comments.Where(m => m.Status == status);

                return query.Select(m => new CommentModel()
                {
                    Id = m.Id,
                    Name = m.Name,
                    InsertDate = m.InsertDate,
                    Type = m.Type,
                    Employee = m.Employee,
                }).OrderByDescending(m => m.Id).ToList();
            }
        }

        public MethodResult Create(CommentModel model)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var comment = new Comment()
                {
                    Type = model.Type,
                    Name = model.Name,
                    Text = model.Text,
                    InsertDate = model.InsertDate,
                    Status = model.Status,
                    Employee = model.Employee,
                };

                context.Comments.InsertOnSubmit(comment);
                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult()
                {
                    IsSuccess = true,
                };
            }
        }

        public CommentModel GetComment(int id)
        {
            using (var transactionScope = this.GetReadOnlyTransactionScope())
            using (var context = this.GetReadOnlyBaseDataContext())
            {
                var comment = context.Comments
                    .Where(m => m.Id == id)
                    .Select(m => new CommentModel()
                    {
                        Id = m.Id,
                        Name = m.Name,
                        InsertDate = m.InsertDate,
                        Type = m.Type,
                        Status = m.Status,
                        Text = m.Text,
                        Employee = m.Employee,
                        Answer = m.Answer,
                    }).FirstOrDefault();

                return comment;
            }
        }

        public MethodResult Close(int id, string answer)
        {
            using (var transactionScope = this.GetWritableTransactionScope())
            using (var context = this.GetWritableBaseDataContext())
            {
                var comment = context.Comments.Where(m => m.Id == id).FirstOrDefault();
                if (comment != null)
                {
                    comment.Status = CommentStatus.Closed;
                    comment.Answer = answer;
                }

                context.TrySubmitChanges();
                transactionScope.Complete();

                return new MethodResult()
                {
                    IsSuccess = true,
                };
            }
        }
    }
}