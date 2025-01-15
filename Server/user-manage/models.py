from sqlalchemy import Column, Integer, String, DateTime, ForeignKey, sql
from sqlalchemy.orm import relationship
from database import Base

class User(Base):
    __tablename__ = 'user'
    
    id = Column(Integer, primary_key=True)
    platform_type = Column(Integer, nullable=False)
    platform_id = Column(String(255), nullable=False)
    created_at = Column(DateTime, server_default=sql.func.now())
    
    # relationship 설정
    tokens = relationship("UserToken", back_populates="user")

class UserToken(Base):
    __tablename__ = 'user_token'
    
    id = Column(Integer, primary_key=True)
    user_id = Column(Integer, ForeignKey('user.id'), nullable=False)
    token = Column(String(255), nullable=False)
    created_at = Column(DateTime, server_default=sql.func.now())
    expire_at = Column(DateTime)
    
    # relationship 설정
    user = relationship("User", back_populates="tokens") 